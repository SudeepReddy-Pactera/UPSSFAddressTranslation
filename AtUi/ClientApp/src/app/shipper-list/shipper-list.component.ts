import { Component, ViewChild, OnInit, Input } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatDialog, MatSort } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
import { ShippingService } from '../services/shipping.service';
import { ShipperListService } from '../services/ShipperListService';
import { ActivatedRoute } from '@angular/router';
import { DataService } from '../services/data.service';
import { Observable } from 'rxjs';
import { ShipperListModelComponent } from './shipper-list-model/shipper-list-model.component';
import { NotificationService } from '../services/NotificationService';
import { DialogService } from '../services/dialog.service';

@Component({
  selector: 'app-shipper-list',
  templateUrl: './shipper-list.component.html',
  styleUrls: ['./shipper-list.component.css']
})
export class ShipperListComponent implements OnInit {

  displayedColumns =
    ['select','actions', 'spC_PSL_CD_TE', 'spC_CTY_TE', 'spC_CTR_TE', 'spC_CPY_TE', 'spC_NA', 'spC_SND_PTY_CTC_TE', 'spC_ADR_TE', 'spC_CTC_PH', 'spC_SLIC_NR', 'spC_CST_ID_TE'];

  private eventsSubscription: any;
  @Input() events: Observable<void>;

  public ResponseData: any[] = [];
  dataSource = new MatTableDataSource<Element>();
  public errorMessage: string;
  selection = new SelectionModel<any>(true, []);
  filterText: string = '';

  constructor(private shippingService: ShippingService, private activatedRoute: ActivatedRoute,
    public dialog: MatDialog, public dataService: DataService, private dialogService: DialogService,
    private shipperListService: ShipperListService, private notificationService: NotificationService) {
  }

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  /**
  * Set the paginator after the view init since this component will
  * be able to query its view for the initialized paginator.
  */
  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  ngOnInit() {
    this.getShipperListData();
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.filteredData.length;
    return numSelected === numRows;
  }

  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.filteredData.forEach(row => this.selection.select(row));
  }

  /** The label for the checkbox on the passed row */
  checkboxLabel(row?: any): string {
    if (!row) {
      return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.position + 1}`;
  }

  rowDelete(index: number, rowData: any) {

  }
  
  getShipperListData() {
    this.ResponseData = [];
    this.shipperListService.getShipperListData().subscribe((response: any) => {
      if (response) {
        if (response.success === true) {
          this.ResponseData = response.shipperCompanies;
        } else {
          this.ResponseData = [];
        }
        this.dataSource.data = this.ResponseData;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.filterText = '';
        this.applyFilter('');
        this.selection.clear();
      }
    }, error => (this.errorMessage = <any>error));
  }

  applyFilter(filterValue: string) {
    this.filterText = filterValue;
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
    this.selection.clear();
  }

  addNew() {
    const dialogRef = this.dialog.open(ShipperListModelComponent, { data: { NEW: true} });

    dialogRef.afterClosed().subscribe(result => {
      //if (result === 1) {
        let NewDetails = this.dataService.getDialogData();

        //const data = {
        //  SPC_PSL_CD_TE: NewDetails.PostalCode,
        //  SPC_CTY_TE: NewDetails.City,
        //  SPC_CTR_TE: NewDetails.Centre,
        //  SPC_CPY_TE: NewDetails.ShippingCompany,
        //  SPC_NA: NewDetails.ShippingName,
        //  SPC_SND_PTY_CTC_TE: NewDetails.SendingPartyContact,
        //  SPC_ADR_TE: NewDetails.ShippingAddress,
        //  SPC_CTC_PH: NewDetails.Contact,
        //  SPC_SLIC_NR: NewDetails.SLICNumber,
        //  SPC_CST_ID_TE: NewDetails.CustomerID
        //}
        //this.shipperListService.addShipperData(data).subscribe((response: any) => {
        //  if (response) {
        //    if (response.success === true) {
        //      this.notificationService.openSuccessMessageNotification("Data Added Successfully.");
        //      this.getShipperListData();
        //    } else {
        //      this.notificationService.openErrorMessageNotification(response.operatonExceptionMessage);
        //    }
        //  } else {
        //    this.notificationService.openErrorMessageNotification("Invalid exception occured, please contact administrator.");
        //  }

        //},
        //  error => this.notificationService.openErrorMessageNotification(error.status + ' : ' + error.statusText))
      if (NewDetails) {
        if (NewDetails.response) {
          if (NewDetails.response.success === true) {
            this.getShipperListData();
          }
        }
      }
        
      //}
    });
  }

  startEdit(i: number, shipperData: any) {
    let ShipperDetails = shipperData;
    const dialogRef = this.dialog.open(ShipperListModelComponent, {
      data: {
        ID: shipperData.id,
        PostalCode: shipperData.spC_PSL_CD_TE,
        City: shipperData.spC_CTY_TE,
        Centre: shipperData.spC_CTR_TE,
        ShippingCompany: shipperData.spC_CPY_TE,
        ShippingName: shipperData.spC_NA,
        SendingPartyContact: shipperData.spC_SND_PTY_CTC_TE,
        ShippingAddress: shipperData.spC_ADR_TE,
        Contact: shipperData.spC_CTC_PH,
        SLICNumber: shipperData.spC_SLIC_NR,
        CustomerID: shipperData.spC_CST_ID_TE,
        NEW: false,
        originalData: {
          ID: shipperData.id,
          PostalCode: shipperData.spC_PSL_CD_TE,
          City: shipperData.spC_CTY_TE,
          Centre: shipperData.spC_CTR_TE,
          ShippingCompany: shipperData.spC_CPY_TE,
          ShippingName: shipperData.spC_NA,
          SendingPartyContact: shipperData.spC_SND_PTY_CTC_TE,
          ShippingAddress: shipperData.spC_ADR_TE,
          Contact: shipperData.spC_CTC_PH,
          SLICNumber: shipperData.spC_SLIC_NR,
          CustomerID: shipperData.spC_CST_ID_TE
        }
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      //if (result === 1) {
        let updatedDetails = this.dataService.getDialogData();

        //if (updatedDetails.PostalCode == shipperData.spC_PSL_CD_TE &&
        //  updatedDetails.City == shipperData.spC_CTY_TE &&
        //  updatedDetails.Centre == shipperData.spC_CTR_TE &&
        //  updatedDetails.ShippingCompany == shipperData.spC_CPY_TE &&
        //  updatedDetails.ShippingName == shipperData.spC_NA &&
        //  updatedDetails.SendingPartyContact == shipperData.spC_SND_PTY_CTC_TE &&
        //  updatedDetails.ShippingAddress == shipperData.spC_ADR_TE &&
        //  updatedDetails.Contact == shipperData.spC_CTC_PH &&
        //  updatedDetails.SLICNumber == shipperData.spC_SLIC_NR &&
        //  updatedDetails.CustomerID == shipperData.spC_CST_ID_TE
        //    ) {

        //  this.notificationService.openSuccessMessageNotification("No changes found to update");
        //  return;
        //}

        //const details = {
        //  ID: ShipperDetails.id,
        //  SPC_PSL_CD_TE: updatedDetails.PostalCode,
        //  SPC_CTY_TE: updatedDetails.City,
        //  SPC_CTR_TE: updatedDetails.Centre,
        //  SPC_CPY_TE: updatedDetails.ShippingCompany,
        //  SPC_NA: updatedDetails.ShippingName,
        //  SPC_SND_PTY_CTC_TE: updatedDetails.SendingPartyContact,
        //  SPC_ADR_TE: updatedDetails.ShippingAddress,
        //  SPC_CTC_PH: updatedDetails.Contact,
        //  SPC_SLIC_NR: updatedDetails.SLICNumber,
        //  SPC_CST_ID_TE: updatedDetails.CustomerID
        //}

        //this.shipperListService.updateShipperList([details]).subscribe((response: any) => {
        //  if (response) {
        //    if (response.success === true) {
        //      shipperData.spC_PSL_CD_TE = response.shipperCompanies[0].spC_PSL_CD_TE,
        //        shipperData.spC_CTY_TE = response.shipperCompanies[0].spC_CTY_TE,
        //        shipperData.spC_CTR_TE = response.shipperCompanies[0].spC_CTR_TE,
        //        shipperData.spC_CPY_TE = response.shipperCompanies[0].spC_CPY_TE,
        //        shipperData.spC_NA = response.shipperCompanies[0].spC_NA,
        //        shipperData.spC_SND_PTY_CTC_TE = response.shipperCompanies[0].spC_SND_PTY_CTC_TE,
        //        shipperData.spC_ADR_TE = response.shipperCompanies[0].spC_ADR_TE,
        //        shipperData.spC_CTC_PH = response.shipperCompanies[0].spC_CTC_PH,
        //        shipperData.spC_SLIC_NR = response.shipperCompanies[0].spC_SLIC_NR,
        //        shipperData.spC_CST_ID_TE = response.shipperCompanies[0].spC_CST_ID_TE
        //      this.notificationService.openSuccessMessageNotification("Data Updated Successfully.");
        //    } else {
        //      this.notificationService.openErrorMessageNotification(response.operatonExceptionMessage);
        //    }
        //  } else {
        //    this.notificationService.openErrorMessageNotification("Invalid exception occured, please contact administrator.");
        //  }

        //},
        //  error => this.notificationService.openErrorMessageNotification(error.status + ' : ' + error.statusText))
      if (updatedDetails) {
        if (updatedDetails.response) {
          if (updatedDetails.response.success === true) {
            shipperData.spC_PSL_CD_TE = updatedDetails.response.shipperCompanies[0].spC_PSL_CD_TE,
              shipperData.spC_CTY_TE = updatedDetails.response.shipperCompanies[0].spC_CTY_TE,
              shipperData.spC_CTR_TE = updatedDetails.response.shipperCompanies[0].spC_CTR_TE,
              shipperData.spC_CPY_TE = updatedDetails.response.shipperCompanies[0].spC_CPY_TE,
              shipperData.spC_NA = updatedDetails.response.shipperCompanies[0].spC_NA,
              shipperData.spC_SND_PTY_CTC_TE = updatedDetails.response.shipperCompanies[0].spC_SND_PTY_CTC_TE,
              shipperData.spC_ADR_TE = updatedDetails.response.shipperCompanies[0].spC_ADR_TE,
              shipperData.spC_CTC_PH = updatedDetails.response.shipperCompanies[0].spC_CTC_PH,
              shipperData.spC_SLIC_NR = updatedDetails.response.shipperCompanies[0].spC_SLIC_NR,
              shipperData.spC_CST_ID_TE = updatedDetails.response.shipperCompanies[0].spC_CST_ID_TE
          }
        }
      }
        
      //}
    });
  }

  delete() {
    const checkedCount = this.selection.selected.length;
    if (checkedCount <= 0) {
      this.dialogService.openAlertDialog('Please select minimum one row to Delete.');
    } else {
      const dialogRef = this.dialogService.openConfirmationDialog('Are you sure, you want to delete all the selected records ?');

      dialogRef.afterClosed().subscribe(data => {
        if (data === true) {
          const dataForDelete = this.selection.selected; // Any changes can do here for sending array
          this.deleteShipmentData(dataForDelete);
        } else {

        }
      })
    }
  }

  deleteShipmentData(data: any) {
    this.shipperListService.deleteShipperList(data).subscribe((response: any) => {
      if (response != null && response.success === true) {
        this.getShipperListData();
        this.notificationService.openSuccessMessageNotification("Deleted Successfully");
      } else {
        this.notificationService.openErrorMessageNotification("Invalid exception occured, please contact administrator.");
      }
    },
      error => this.notificationService.openErrorMessageNotification(error.status + ' : ' + error.statusText));
  }
}
