import { Component, OnInit, ViewChild } from '@angular/core';
import { ReportsService } from '../../services/ReportsService';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { LoaderService } from '../../shared/loader/loader.service';
import { NotificationService } from '../../services/NotificationService';
import { ExcelService } from '../../services/ExcelExport';
import { DialogService } from '../../services/dialog.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.css']
})
export class SummaryComponent implements OnInit {
  public tableData: any[] = [];
  public excelMainData: any[] = [];
  fromDate: any;
  toDate: any;
  filterText: string = '';
  currentDate = new Date();
 // displayedColumns = ['wfL_ID', 'flE_NA', 'usR_FST_NA', 'toT_UP_ADR', 'toT_TR_ADR', 'toT_TR_ADR_PER', 'tR_FRM_ADR_BK', 'tR_FRM_ADR_BK_PER', 'tR_FRM_TR_TOOL', 'tR_FRM_TR_TOOL_PER', 'adR_MOD', 'adR_MOD_PER', 'crD_DT'];
  displayedColumns = ['wfL_ID', 'usR_FST_NA', 'ctY_TXT', 'crD_DT','toT_UP_ADR', 'toT_TR_ADR', 'toT_TR_ADR_PER', 'tR_FRM_ADR_BK', 'tR_FRM_ADR_BK_PER', 'tR_FRM_TR_TOOL', 'tR_FRM_TR_TOOL_PER', 'adR_MOD', 'adR_MOD_PER'];
  dataSource = new MatTableDataSource<Element>();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  constructor(private reportsService: ReportsService,
    private dialogService: DialogService,
    private notificationService: NotificationService,
    private excelService: ExcelService,
    private datepipe: DatePipe) {
    this.fromDate = this.datepipe.transform(this.currentDate, 'yyyy-MM-dd');
    this.toDate = this.datepipe.transform(this.currentDate, 'yyyy-MM-dd');
  }
  ngOnInit() {
    //this.getSummaryData();
    console.log(this.currentDate);
    this.getSummaryData();  
  
  }
  applyFilter(filterValue: string) {
    this.filterText = filterValue;
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }


  onDateFilter() {
    const FROM = (<HTMLInputElement>document.getElementById('fromDate')).value;
    const TO = (<HTMLInputElement>document.getElementById('toDate')).value;
    if (FROM && TO) {
      const fromdate = this.datepipe.transform(FROM, 'yyyy-MM-dd');
      const todate = this.datepipe.transform(TO, 'yyyy-MM-dd');
      if (fromdate > todate) {
        this.dialogService.openAlertDialog('Invalid dates for search.');
      } else {
        this.applyFilter('');

        this.reportsService.getSummaryData(fromdate, todate)
          .subscribe((data: any) => {
            console.log(data);
            this.dataSource.data = data;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;

          },
            error => console.log(error));

        
      }
    } else {
      this.dialogService.openAlertDialog('Please select From and To dates to search.');
    }
  }

  onResetDateSearch() {
    this.applyFilter('');
    this.fromDate = this.datepipe.transform(this.currentDate, 'yyyy-MM-dd');
    this.toDate = this.datepipe.transform(this.currentDate, 'yyyy-MM-dd');


    //this.dataSource.data = this.ResponseData;
  }
  getSummaryData() {
   

    this.reportsService.getSummaryData(this.datepipe.transform(this.currentDate, 'yyyy-MM-dd'), this.datepipe.transform(this.currentDate, 'yyyy-MM-dd'))
          .subscribe((data: any) => {

            this.dataSource.data = data;
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;

          },
            error => console.log(error));
  }
  exportToExcel() {
    this.tableData = [];
    this.excelMainData = [];
    this.tableData = this.dataSource.sortData(this.dataSource.filteredData, this.dataSource.sort);
    if (this.tableData.length > 0) {
      for (let data of this.tableData) {
        this.excelMainData.push(
          {
            'Workflow ID': data.wfL_ID,
            'Requester': data.usR_FST_NA,
            'City': data.ctY_TXT,
            'Created Date':this.datepipe.transform(data.crD_DT, 'MMMM dd, yyyy hh:mm a'),
            'Total Uploaded Address': data.toT_UP_ADR,
            'Total Translated Address': data.toT_TR_ADR,
            'Total Translated Address(%)': data.toT_TR_ADR_PER,
            'Translated Address from Address Book': data.tR_FRM_ADR_BK,
            'Translated Address from Address Book(%)': data.tR_FRM_ADR_BK_PER,
            'Translated Address from Translation Tool': data.tR_FRM_TR_TOOL,
            'Translated Address from Translation Tool(%)': data.tR_FRM_TR_TOOL_PER,
            'Total Address with Modification': data.adR_MOD,
            'Total Address with Modification(%)': data.adR_MOD_PER,
          })
      }
      this.excelService.exportAsExcelFile(this.excelMainData, 'Summary');
    } else {
      this.dialogService.openAlertDialog('No data for export.');
    }
  }
}
