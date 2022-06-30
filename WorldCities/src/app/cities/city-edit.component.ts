import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { City } from './city';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss'],
})
export class CityEditComponent implements OnInit {
  //view title
  title?: string;
  //form model
  form!: FormGroup;
  //city to edit
  city?: City;
  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.form = new FormGroup({
      name: new FormControl(''),
      lat: new FormControl(''),
      lon: new FormControl(''),
    });
    this.loadData();
  }

  loadData() {
    //retrieve ID from 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    var id = idParam ? +idParam : 0;

    //fetch the city from the backend
    var url = environment.baseUrl + 'api/Cities/' + id;
    this.http.get<City>(url).subscribe({
      next: (result) => {
        this.city = result;
        this.title = `Edit - ${this.city.name}`;

        //update the form with city value
        this.form.patchValue(this.city);
      },
      error: (err) => console.error(err),
    });
  }

  onSubmit() {
    var city = this.city;
    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = this.form.controls['lat'].value;
      city.lon = this.form.controls['lon'].value;

      var url = `${environment.baseUrl}api/Cities/${city.id}`;
      this.http.put<City>(url, city).subscribe({
        next: (result) => {
          console.log(`City ${city!.id} has been updated.`);

          //go back to cities list
          this.router.navigate(['/cities']);
        },
        error: (err) => console.error(err),
      });
    }
  }
}
