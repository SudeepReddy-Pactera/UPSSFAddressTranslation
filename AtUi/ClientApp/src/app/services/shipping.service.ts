import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { HttpService } from '../shared/http.service';
import { ShipmentDetails } from '../models/shipmentDetails';
import { shipmentStatus } from '../shared/enums.service';

@Injectable()
export class ShippingService {
  constructor(private httpClient: HttpClient, private router: Router,
    private httpService: HttpService) { }

  public getUploadedData(WorkflowID: any): Observable<any> {
    return this.httpService.makeGetRequest('api/Shipment/GetAllShipmentData?wid=' + WorkflowID);
  }

  public getAPIType(): Observable<any> {
    return this.httpService.makeGetRequest('api/Shipment/GetAPIType');
  }
  
  public getTranslateData(WorkflowID: any): Observable<any> {
    return this.httpService.makeGetRequest('api/Shipment/GetShipmentData?wid=' + WorkflowID);
  }

  public getTranslateDataByCity(WorkflowID: any, apiType): Observable<any> {
    return this.httpService.makeGetRequest('api/Shipment/GetShipmentDataByCity?wid=' + WorkflowID + '&apiType=' + apiType);
  }

  public getDataForSendToSF(WorkflowID: any): Observable<any> {
    return this.httpService.makeGetRequest('api/Shipment/GetMatchedShipmentsWithShipperCompanies?wid=' + WorkflowID);
  }

  public getCompletedShipments(WorkflowID: any): Observable<any> {
    return this.httpService.makeGetRequest('api/Shipment/GetCompletedShipments?wid=' + WorkflowID);
  }

  public sendDataForTranslate(data: any): Observable<any> {
    return this.httpService.makePostRequest('api/Shipment/GetTranslationAddress', data);  // Add URL here for send for translate
  }

  public sendDataForSFTranslate(data: any): Observable<any> {
    return this.httpService.makePostRequest('api/Shipment/GetSFTranslationAddress', data);  // Add URL here for send for translate
  }

  public sendDataToSF(data: any): Observable<any> {
    return this.httpService.makePostRequest('api/Shipment/CreateOrderShipment', data);
  }

  public UpdateShippingAddress(data: ShipmentDetails): Observable<ShipmentDetails> {
    //var user = localStorage.getItem("userid");
    return this.httpService.makePostRequest('api/Shipment/UpdateShipmentAddressById', data);
  }

  public deleteUploadedData(data: any): Observable<any> {
    return this.httpService.makePostRequest('api/Shipment/DeleteShipments', data);
  }
}
