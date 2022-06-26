import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { environment } from '../../environments/environment.prod';
import { City } from './city';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss'],
})
export class CitiesComponent implements OnInit {
  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon'];
  public cities!: MatTableDataSource<City>;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<City[]>(environment.baseUrl + 'api/cities').subscribe(
      (result) => {
        this.cities = new MatTableDataSource<City>(result);
        this.cities.paginator = this.paginator;
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
