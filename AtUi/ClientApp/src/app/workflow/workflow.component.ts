import { Component, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSnackBar, MatSnackBarConfig, MatProgressSpinner, MatSort } from '@angular/material';
import { UserService } from '../services/UserService';
import { Http, RequestOptions, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Rx';  
import * as XLSX from 'xlsx';
import { FormControl } from '@angular/forms';
import { LoaderService } from '../shared/loader/loader.service';
import { List } from 'linq-typescript';
import { Constants } from '../shared/Constants';
import { NotificationService } from '../services/NotificationService';
import { WorkflowService } from '../services/WorkflowService';
import { ExcelService } from '../services/ExcelExport';

@Component({
  selector: 'workflow',
  styleUrls: ['workflow.component.css'],
  templateUrl: 'workflow.component.html',
})

export class WorkflowComponent {

  arrayBuffer: any;
  file: File;
  fileToUpload: File = null;
  displayedColumns = ['id', 'usR_FST_NA', 'flE_NA', 'wfL_STA_TE_TEXT', 'crD_DT'];
  dataSource = new MatTableDataSource<Element>();
  filterText: string;
  public excelMainData: any[] = [];

  fileNameControl = new FormControl('');
  isValidFile: boolean = true;
   workFlowStatus=[
  { key: 1, value: 'InProgress' },
     { key: 2, value: 'Uploaded' }
]; // create an empty array

  constructor(
    private _loaderService: LoaderService,
    private snackBar: MatSnackBar,
      private notificationService: NotificationService,
    private workflowService: WorkflowService,
    private excelService: ExcelService
  ) {

  }

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  ngOnInit() {
  
    this.getWorkflowDetails();
} 

  getWorkflowDetails() {
    var user = localStorage.getItem("Emp_Id");
    this.workflowService.GetAllWorkflow()
      .subscribe((data: any) => {

        this.dataSource.data = data;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;

      },
        error => console.log(error));
  }
  
  applyFilter(filterValue: string) {
    this.filterText = filterValue;
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  onFileChange(evt: any) {
    /* wire up file reader */
    const target: DataTransfer = <DataTransfer>(evt.target);
    if (target.files.length !== 1) throw new Error('Cannot use multiple files');
    const reader: FileReader = new FileReader();
    reader.onload = (e: any) => {
      /* read workbook */
      const bstr: string = e.target.result;
      const wb: XLSX.WorkBook = XLSX.read(bstr, { type: 'binary' });

      /* grab first sheet */
      const wsname: string = wb.SheetNames[0];
      const ws: XLSX.WorkSheet = wb.Sheets[wsname];

      /* save data */
      //this.data = <AOA>(XLSX.utils.sheet_to_json(ws, { header: 1 }));
    };
    reader.readAsText(target.files[0]);
  }

  handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);
    let fileName = this.fileToUpload.name;
    this.fileNameControl.setValue(fileName);
    if (!this.validateFile(fileName)) {
      this.isValidFile = false;
      return;
    }
    else {
      this.isValidFile = true;
      this.workflowService.UploadFile(this.fileToUpload)
        .subscribe((response: any) => {
          if (response.success === true) {
            this.getWorkflowDetails();
            this.exportFailedExcelData(response.excelFailedData, fileName);
            this.notificationService.openSuccessMessageNotification("File Uploaded successfully ," + response.shipments.length + " rows inserted and " + response.excelFailedData.length+" rows rejected");
            this.resetFileUpload();
          } else if (response.success === false) {
            if (response.exception) {
              this.notificationService.openErrorMessageNotification(response.exception.Message);
              this.resetFileUpload();
            }
            else {
              this.notificationService.openErrorMessageNotification('Low Bandwidth . Please re-upload file');
              this.resetFileUpload();
            }
          }
        },
        error =>
        {
          this.notificationService.openErrorMessageNotification(error.status + ' : ' + error.statusText);
          this.resetFileUpload();
        }
      );
      this.applyFilter('');
      this.filterText = '';
    }
  }
  validateFile(name: String) {
    var ext = name.substring(name.lastIndexOf('.') + 1);
    if (ext.toLowerCase() == 'xlsx' || ext.toLowerCase() == 'xls') {
      return true;
    }
    else {
      return false;
    }
  }

  resetFileUpload() {
    this.fileNameControl.setValue('');
    (<HTMLInputElement>document.getElementById('file')).value = '';
  }

  exportFailedExcelData(failedExcelData: any,filename:string) {
    this.excelMainData = [];
    filename = filename + "-ErrorData";
    if (failedExcelData.length > 0) {
      for (let data of failedExcelData) {
        this.excelMainData.push(
          {
            'shipment no': data.s_shipmentno,
            'pcs': Math.floor(data.pcs),
            'package no': data.s_packageno,
            'pkg wei.': data.s_pkgwei,
            'shpt wei.': data.s_shptwei,
            'dim. wei.': data.s_dimwei,
            'wei. unit': data.s_dimwei,
            'svl': Math.floor(data.svl),
            'pymt': data.pymt,
            'ship date': data.s_shipdate,
            'pkup time': data.s_pkuptime,
            'bill type': data.s_billtype,
            'value': Math.floor(data.value),
            'currency': data.currency,
            '1st invoice line desc.': data.s_1stinvoicelinedesc,
            'exp slic': data.s_expslic,
            'shpr#': data.s_shpr,
            'shipper company': data.s_shippercompany,
            'address': data.address,
            'org city': data.s_orgcity,
            'org psl': data.s_orgpsl,
            ' shpt ctc': data.s_shptctc,
            'shpt ph#': data.s_shptph,
            'imp slic': data.s_impslic,
            'impr#': data.s_impr,
            'receiver company': data.s_receivercompany,
            'address1': data.s_address1,
            'dst city': data.s_dstcity,
            'dst psl': data.s_dstpsl,
            'cnee ctc': data.s_cneectc,
            'ph#': data.s_ph,
            'in flight': data.s_inflight,
            'out flight': data.s_outflight,
            'error message': data.s_ExceptionMessage,

          })
      }
      this.excelService.exportAsErrorExcelFile(this.excelMainData, filename);
    }

  }
 
}

export interface Element {
  id: number;
  usR_FST_NA: string;
  
  //udT_DT: string;
  flE_NA: string;
  wfL_STA_TE_TEXT: string;
  crD_DT: string;
 
}


