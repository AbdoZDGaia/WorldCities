import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResult, BaseService } from '../base.service';
import { Country } from '../countries/country';
import { City } from './city';

@Injectable({
  providedIn: 'root',
})
export class CityService extends BaseService<City> {
  getData(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null
  ): Observable<ApiResult<City>> {
    var url = this.getUrl('api/cities');
    var params = new HttpParams()
      .set('pageIndex', pageIndex.toString())
      .set('pageSize', pageSize.toString())
      .set('sortColumn', sortColumn)
      .set('sortOrder', sortOrder);

    if (filterColumn && filterQuery) {
      params = params
        .set('filterColumn', filterColumn)
        .set('filterQuery', filterQuery);
    }

    return this.http.get<ApiResult<City>>(url, { params });
  }
  get(id: number): Observable<City> {
    var url = this.getUrl('api/Cities/' + id);
    return this.http.get<City>(url);
  }
  put(item: City): Observable<City> {
    var url = this.getUrl('api/Cities/' + item.id);
    return this.http.put<City>(url, item);
  }
  post(item: City): Observable<City> {
    var url = this.getUrl('api/Cities');
    return this.http.post<City>(url, item);
  }

  getCountries(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null
  ): Observable<ApiResult<Country>> {
    var url = this.getUrl('api/Countries');
    var params = new HttpParams()
      .set('pageIndex', pageIndex.toString())
      .set('pageSize', pageSize.toString())
      .set('sortColumn', sortColumn)
      .set('sortOrder', sortOrder);
    if (filterColumn && filterQuery) {
      params = params
        .set('filterColumn', filterColumn)
        .set('filterQuery', filterQuery);
    }
    return this.http.get<ApiResult<Country>>(url, { params });
  }

  isDupeCity(item: City): Observable<boolean> {
    var url = this.getUrl('api/Cities/isDupeCity');
    return this.http.post<boolean>(url, item);
  }

  constructor(http: HttpClient) {
    super(http);
  }
}
