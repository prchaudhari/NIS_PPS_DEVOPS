
import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ScheduleLogServiceDetail } from './logs-details.service';
import { ScheduleLogDetail } from './log-details';
import { PreviewDialogService } from './../../shared/services/preview-dialog.service';
import { AppSettings } from '../../appsettings';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import * as $ from 'jquery';

export interface ListElement {
  UserID: string;
  status: string;
  date: string;
}

@Component({
  selector: 'app-logs-details',
  templateUrl: './logs-details.component.html',
  styleUrls: ['./logs-details.component.scss']
})
export class LogsDetailsComponent implements OnInit {

  //public variables
  public isFilter: boolean = false;
  public scheduleLogList: ScheduleLogDetail[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 20;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedScheduleLogList: ScheduleLogDetail[] = [];
  public pageTypeList: any[] = [];
  public ScheduleLogFilterForm: FormGroup;
  public filterPageStatus: string = '';
  public userClaimsRolePrivilegeOperations: any[] = [];
  public scheduleLogIdentifier;
  public params: any = [];
  public isCheckAll: boolean = false
  public disableMultipleDelete = true;
  public scheduleName: string;
  public executionDate;
  public isAllRecordSuccess = true;
  public BatchStatus: string;

  public totalRecordCount = 0;
  public filterUserIdValue = '';
  public filterLogStatus = '';
  public sortColumn = 'Name';
  public sortOrder = Constants.Ascending;
  public baseURL = AppSettings.baseURL;

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  displayedColumns: string[] = ['UserID', 'renderEngineName', 'status', 'date', 'actions'];
  dataSource = new MatTableDataSource<any>();

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  public DataFormat;
  ngOnInit() {
    this.DataFormat = localStorage.getItem('DateFormat');
    this.getScheduleLogs(null);
    //this.getPageTypes();
    this.ScheduleLogFilterForm = this.fb.group({
      filterUserId: [null],
      filterStatus: [0],
      filterPublishedOnFromDate: [null],
      filterPublishedOnToDate: [null],
    });

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.userClaimsRolePrivilegeOperations = [];
    }
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  navigateToListPage() {
    this.route.navigate(['/logs']);
  }

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
    private localstorageservice: LocalStorageService,
    private scheduleLogService: ScheduleLogServiceDetail,
    private http: HttpClient) {
    this.sortedScheduleLogList = this.scheduleLogList.slice();

    route.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/logsDetails')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('scheduleLogParams'));
          if (localStorage.getItem('scheduleLogParams')) {
            this.scheduleLogIdentifier = this.params.Routeparams.passingparams.SchdeuleLogIdetifiier;
            this.scheduleName = this.params.Routeparams.passingparams.SchdeuleName;
            this.executionDate = this.params.Routeparams.passingparams.ExecutionDate;
            this.BatchStatus = this.params.Routeparams.passingparams.BatchStatus;
          }
        } else {
          localStorage.removeItem("scheduleLogParams");
        }
      }

    });
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getScheduleLogs(null);
  }

  //Getters for Page Forms
  get filterUserId() {
    return this.ScheduleLogFilterForm.get('filterUserId');
  }

  get filterStatus() {
    return this.ScheduleLogFilterForm.get('filterStatus');
  }

  sortData(sort: MatSort) {
    const data = this.scheduleLogList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedScheduleLogList = data;
      return;
    }

    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    }
    else {
      this.sortOrder = Constants.Descending;
    }
    
    if (sort.active == 'UserID') {
      this.sortColumn = 'CustomerName';
    }
    else if (sort.active == 'renderEngineName') {
      this.sortColumn = 'RenderEngineName';
    }
    else if (sort.active == 'date') {
      this.sortColumn = 'CreationDate';
    }
    else if (sort.active == 'status') {
      this.sortColumn = 'Status';
    }
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    if (this.ScheduleLogFilterForm.value.filterUserId != null && this.ScheduleLogFilterForm.value.filterUserId != '') {
      this.filterUserIdValue = this.ScheduleLogFilterForm.value.filterUserId.trim();
      searchParameter.CustomerName = this.ScheduleLogFilterForm.value.filterUserId.trim();
    }
    if (this.ScheduleLogFilterForm.value.filterStatus != null && this.ScheduleLogFilterForm.value.filterStatus != 0) {
      this.filterLogStatus = this.ScheduleLogFilterForm.value.filterStatus;
      searchParameter.Status = this.ScheduleLogFilterForm.value.filterStatus;
    }
    this.getScheduleLogs(searchParameter);
  }

  async getScheduleLogs(searchParameter) {
    let scheduleLogService = this.injector.get(ScheduleLogServiceDetail);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = 'Id';
      searchParameter.SortParameter.SortOrder = Constants.Descending;
      searchParameter.SearchMode = Constants.Contains;
      if (this.filterUserIdValue != null && this.filterUserIdValue != '') {
        searchParameter.CustomerName = this.filterUserIdValue.trim();
      }
      if (this.filterLogStatus != null && this.filterLogStatus != '') {
        searchParameter.Status = this.filterLogStatus;
      }
    }
    searchParameter.ScheduleLogId = this.scheduleLogIdentifier
    var response = await scheduleLogService.getScheduleLogDetail(searchParameter);
    this.scheduleLogList = response.List;
    this.totalRecordCount = response.RecordCount;
    if(this.scheduleLogList.length > 0){
      var records = this.scheduleLogList.filter(x  => x.Status != 'Completed');
      if(records.length > 0) {
        this.isAllRecordSuccess = false;
      }
    }

    if (this.scheduleLogList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetSchdeuleLogFilterForm();
          this.getScheduleLogs(null);
        }
      });
    }
    this.dataSource = new MatTableDataSource<ScheduleLogDetail>(this.scheduleLogList);
    this.dataSource.sort = this.sort;
    this.array = this.scheduleLogList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  //This method has been used for fetching search records
  searchScheduleLogRecordFilter(searchType) {
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetSchdeuleLogFilterForm();
      this.getScheduleLogs(null);
      this.isFilter = !this.isFilter;
    }
    else {
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = 'Id';
      searchParameter.SortParameter.SortOrder = Constants.Descending;
      searchParameter.SearchMode = Constants.Contains;
      
      if (this.ScheduleLogFilterForm.value.filterUserId != null && this.ScheduleLogFilterForm.value.filterUserId != '') {
        this.filterUserIdValue = this.ScheduleLogFilterForm.value.filterUserId.trim();
        searchParameter.CustomerName = this.ScheduleLogFilterForm.value.filterUserId.trim();
      }
      if (this.ScheduleLogFilterForm.value.filterStatus != null && this.ScheduleLogFilterForm.value.filterStatus != 0) {
        this.filterLogStatus = this.ScheduleLogFilterForm.value.filterStatus;
        searchParameter.Status = this.ScheduleLogFilterForm.value.filterStatus;
      }

      this.currentPage = 0;
      this.getScheduleLogs(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  resetSchdeuleLogFilterForm() {
    this.ScheduleLogFilterForm.patchValue({
      filterUserId: null,
      filterStatus: 0,
    });
    this.currentPage = 0;
    this.filterUserIdValue = '';
    this.filterLogStatus = '';
  }

  //function written to retry failed customers log
  reTryAllLog() {
    var logDetailList = [];
    this.scheduleLogList.forEach((item, index) => {
      if (item.IsChecked) {
        logDetailList.push(item);
      }

    });
    if (logDetailList == null || logDetailList.length == 0) {
      this._messageDialogService.openDialogBox('Message', "Please select log details which you want to retry.", Constants.msgBoxError);
    }
    else {
      let message = 'Are you sure, you want to retry this schedule log details?';
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          let resultflag = await this.scheduleLogService.reRunSchdeulLogDetail(logDetailList);
          if (resultflag) {
            let messageString = Constants.StatementGeneratedSuccessfullyForSelectedRecordMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.getScheduleLogs(null);
          }
        }
      });
    }
  }

  //function written to retry failed customer log
  reTryLog(log: ScheduleLogDetail) {
    let message = 'Are you sure, you want to run this schedule?';
    var logDetailList = [];
    logDetailList.push(log);
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let resultflag = await this.scheduleLogService.reRunSchdeulLogDetail(logDetailList);
        if (resultflag) {
          let messageString = Constants.StatementGeneratedSuccessfullyForSelectedRecordMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.getScheduleLogs(null);
        }
      }
    });
  }

  IsCheckAll(event): void {
    const value = event.checked;
    if (value) {
      this.isCheckAll = true;
      for (let i = 0; i < this.scheduleLogList.length; i++) {
        this.scheduleLogList[i].IsChecked = true;
      }
      this.dataSource = new MatTableDataSource(this.scheduleLogList);
      this.disableMultipleDelete = false;
    }
    else {
      this.isCheckAll = false;
      for (let i = 0; i < this.scheduleLogList.length; i++) {
        this.scheduleLogList[i].IsChecked = false;
      }
      this.disableMultipleDelete = true;
      this.dataSource = new MatTableDataSource(this.scheduleLogList);
    }
  }

  IsCheckItem(event, element): void {
    if (event.checked) {
      this.disableMultipleDelete = false;
      let itemIndex = 0;
      this.scheduleLogList.forEach((item, index) => {
        if (item.Identifier == element.Identifier) {
          itemIndex = index;
        }
      })
      this.scheduleLogList[itemIndex].IsChecked = true;

      let isdisable = false;
      this.scheduleLogList.forEach((item, index) => {
        if (!item.IsChecked) {
          isdisable = true;
        }
      })
      if (!isdisable) {
        this.isCheckAll = true;
      }
    }
    else {
      this.isCheckAll = false;
      this.scheduleLogList.forEach
      let itemIndex = 0;
      this.scheduleLogList.forEach((item, index) => {
        if (item.Identifier == element.Identifier) {
          itemIndex = index;
        }
      });
      this.scheduleLogList[itemIndex].IsChecked = false;
      let isdisable = true;
      this.scheduleLogList.forEach((item, index) => {
        if (item.IsChecked) {
          isdisable = false;
        }
      })
      if (!isdisable) {
        this.disableMultipleDelete = false;
      }
      else {
        this.disableMultipleDelete = true;
      }
    }
  }

  viewLodMessage(log: ScheduleLogDetail) {
    if (log.Status == 'Failed') {
      let previewservice = this.injector.get(PreviewDialogService);
      previewservice.openMulipleMessageDialogBox('Error', log.LogMessage, Constants.msgBoxSuccess);
    }
    else {
      let previewservice = this.injector.get(PreviewDialogService);
      previewservice.openMulipleMessageDialogBox('Success', log.LogMessage, Constants.msgBoxSuccess);
    }
  }

  ViewHTML(log: ScheduleLogDetail) {
    this.uiLoader.start();
    this.http.post(this.baseURL + 'ScheduleLog/ScheduleLogDetail/DownloadHTMLStatement', log, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
      .subscribe(
        data => {
          this.uiLoader.stop();
          let contentType = data.headers.get('Content-Type');
          let fileName = data.headers.get('x-filename');
          fileName = fileName.substring(fileName.lastIndexOf('\\') + 1, fileName.length);
          const blob = new Blob([data.body], { type: contentType });
          if (window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(blob, fileName);
          } else {
            var link = document.createElement('a');
            link.setAttribute("type", "hidden");
            link.download = fileName;
            link.href = window.URL.createObjectURL(blob);
            document.body.appendChild(link);
            link.click();
          }
        },
        error => {
          $('.overlay').show();
          this._messageDialogService.openDialogBox('Error', "File Not Found", Constants.msgBoxError);
          this.uiLoader.stop();
        });
  }

  ExportToPDF(log: ScheduleLogDetail): void {
    this.uiLoader.start();
    this.http.post(this.baseURL + 'ScheduleLog/ScheduleLogDetail/ExportToPDF' , log, { responseType: "arraybuffer", observe: 'response' }).pipe(map(response => response))
      .subscribe(
        data => {
          this.uiLoader.stop();
          let contentType = data.headers.get('Content-Type');
          let fileName = data.headers.get('x-filename');
          fileName = fileName.substring(fileName.lastIndexOf('\\') + 1, fileName.length);
          const blob = new Blob([data.body], { type: contentType });
          if (window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(blob, fileName);
          } else {
            var link = document.createElement('a');
            link.setAttribute("type", "hidden");
            link.download = fileName;
            link.href = window.URL.createObjectURL(blob);
            document.body.appendChild(link);
            link.click();
          }
        },
        error => {
          $('.overlay').show();
          this._messageDialogService.openDialogBox('Error', "File Not Found", Constants.msgBoxError);
          this.uiLoader.stop();
        });
   }
}

