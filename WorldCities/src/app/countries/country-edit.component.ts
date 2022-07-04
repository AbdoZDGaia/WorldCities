import { map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AsyncValidatorFn,
  AbstractControl,
} from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { Country } from './country';
import { BaseFormComponent } from '../base-form.component';

@Component({
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrls: ['./country-edit.component.scss'],
})
export class CountryEditComponent extends BaseFormComponent implements OnInit {
  //view title
  title?: string;

  //country object to create or edit
  country?: Country;

  //country id, as fetched from the active route:
  //it's NULL when we're adding a new country,
  //not NULL when we're editing an existing one.
  id?: number;

  //countries array for the select
  countries?: Country[];

  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {
    super();
  }

  ngOnInit() {
    this.form = this.fb.group({
      name: ['', Validators.required, this.isDupeField('name')],
      iso2: [
        '',
        [Validators.required, Validators.pattern(/^[a-zA-Z]{2}$/)],
        this.isDupeField('iso2'),
      ],
      iso3: [
        '',
        [Validators.required, Validators.pattern(/^[a-zA-Z]{3}$/)],
        this.isDupeField('iso3'),
      ],
    });
    this.loadData();
  }

  loadData() {
    //retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      //EDIT Mode

      //fetch country from the server
      var url = environment.baseUrl + 'api/countries/' + this.id;
      this.http.get<Country>(url).subscribe({
        next: (result) => {
          this.country = result;
          this.title = 'Edit - ' + this.country.name;

          //update the form with the country value
          this.form.patchValue(this.country);
        },
        error: (err) => console.error(err),
      });
    } else {
      //ADD Mode

      this.title = 'Create a new Country';
    }
  }

  onSubmit() {
    var country = this.id ? this.country : <Country>{};
    if (country) {
      country.name = this.form.controls['name'].value;
      country.iso2 = this.form.controls['iso2'].value;
      country.iso3 = this.form.controls['iso3'].value;

      if (this.id) {
        //EDIT Mode

        var url = environment.baseUrl + 'api/countries/' + country.id;
        this.http.put<Country>(url, country).subscribe({
          next: (result) => {
            console.log('Country ' + country!.id + 'has been updated.');

            //go back to countries view
            this.router.navigate(['/countries']);
          },
          error: (err) => console.error(err),
        });
      } else {
        //ADD Mode

        var url = environment.baseUrl + 'api/countries';
        this.http.post<Country>(url, country).subscribe({
          next: (result) => {
            console.log('Country ' + result.id + ' has been created.');

            //go back to countries view
            this.router.navigate(['/countries']);
          },
          error: (err) => console.error(err),
        });
      }
    }
  }

  isDupeField(fieldName: string): AsyncValidatorFn {
    return (
      control: AbstractControl
    ): Observable<{ [key: string]: any } | null> => {
      var params = new HttpParams()
        .set('countryId', this.id ? this.id.toString() : '0')
        .set('fieldName', fieldName)
        .set('fieldValue', control.value);
      var url = environment.baseUrl + 'api/countries/IsDupeField';
      return this.http.post<boolean>(url, null, { params }).pipe(
        map((result) => {
          return result ? { isDupeField: true } : null;
        })
      );
    };
  }
}
