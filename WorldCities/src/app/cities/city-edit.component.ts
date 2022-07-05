import { CityService } from './city.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  AsyncValidatorFn,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { City } from './city';
import { Country } from '../countries/country';
import { map, Observable, Subscription } from 'rxjs';
import { BaseFormComponent } from '../base-form.component';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss'],
})
export class CityEditComponent
  extends BaseFormComponent
  implements OnInit, OnDestroy
{
  //view title
  title?: string;
  //city to edit
  city?: City;

  //city id, as fetched from the active route:
  //it's NULL when we're adding a new city,
  //not NULL when we're editing an existing one.
  id?: number;

  //Activity Log(for debugging purposes)
  activityLog: string = '';

  //The countries observable for the select (using async pipe)
  countries?: Observable<Country[]>;

  private subsriptions: Subscription[] = [];
  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private cityService: CityService
  ) {
    super();
  }

  ngOnInit(): void {
    this.form = new FormGroup(
      {
        name: new FormControl('', Validators.required),
        lat: new FormControl('', [
          Validators.required,
          Validators.pattern(/^[-]?\d+(\.\d{1,4})?$/),
        ]),
        lon: new FormControl('', [
          Validators.required,
          Validators.pattern(/^[-]?\d+(\.\d{1,4})?$/),
        ]),
        countryId: new FormControl('', Validators.required),
      },
      null,
      this.isDupeCity()
    );

    //react to form changes
    this.subsriptions.push(
      this.form.valueChanges.subscribe({
        next: () => {
          if (this.form.dirty) {
            this.log('Form was updated by the user.');
          } else {
            this.log('Form Model has been loaded');
          }
        },
        error: () => console.error('value changes related error'),
      })
    );

    // react to changes in the form.name control
    this.subsriptions.push(
      this.form.get('name')!.valueChanges.subscribe({
        next: () => {
          if (this.form.dirty) {
            this.log('Name was updated by the user.');
          } else {
            this.log('Name has been loaded with initial values.');
          }
        },
        error: () => console.error('value changes related error'),
      })
    );

    this.loadData();
  }

  log(str: string) {
    this.activityLog += `[${new Date().toLocaleString()}] ${str} <br/>`;
  }

  isDupeCity(): AsyncValidatorFn {
    return (
      control: AbstractControl
    ): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};
      city.id = this.id ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lon = this.form.controls['lon'].value;
      city.lat = this.form.controls['lat'].value;
      city.countryId = this.form.controls['countryId'].value;

      return this.cityService.isDupeCity(city).pipe(
        map((result) => {
          return result ? { isDupeCity: true } : null;
        })
      );
    };
  }

  loadData() {
    //load countries
    this.loadCountries();

    //retrieve ID from 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;

    if (this.id) {
      //fetch the city from the backend
      this.cityService.get(this.id).subscribe({
        next: (result) => {
          this.city = result;
          this.title = `Edit - ${this.city.name}`;

          //update the form with city value
          this.form.patchValue(this.city);
        },
        error: (err) => console.error(err),
      });
    } else {
      this.title = 'Create a new City';
    }
  }

  loadCountries() {
    this.countries = this.cityService
      .getCountries(0, 9999, 'name', 'asc', null, null)
      .pipe(map((x) => x.data));
  }

  onSubmit() {
    var city = this.id ? this.city : <City>{};
    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      if (this.id) {
        //EDIT mode
        this.cityService.put(city).subscribe({
          next: (result) => {
            console.log(`City ${city!.id} has been updated.`);

            //go back to cities view
            this.router.navigate(['/cities']);
          },
          error: (err) => console.error(err),
        });
      } else {
        //ADD mode
        this.cityService.post(city).subscribe({
          next: (result) => {
            console.log(`City ${result.id} has been created.`);
            this.router.navigate(['/cities']);
          },
          error: (err) => console.error(err),
        });
      }
    }
  }

  ngOnDestroy() {
    this.subsriptions.forEach((subscription) => subscription.unsubscribe());
  }
}
