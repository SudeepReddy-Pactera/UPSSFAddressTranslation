import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { HttpService } from '../shared/http.service';

@Injectable()
export class ReportsService {
  constructor(private httpClient: HttpClient, private router: Router,
    private httpService: HttpService) { }

  public getSummaryData(fromDate:any,toDate:any): Observable<any> {
    return this.httpService.makeGetRequest('api/Reports/getSummaryData?fromDate=' + fromDate + '&toDate=' + toDate);
  }
}
