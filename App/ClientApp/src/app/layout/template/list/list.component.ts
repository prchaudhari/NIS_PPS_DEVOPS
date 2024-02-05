import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TemplateService } from '../template.service';
import { Template } from '../template';
import { PreviewDialogService } from '../../../shared/services/preview-dialog.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  //public variables
  public isFilter: boolean = false;
  public templateList: Template[] = [];
  public isLoaderActive: boolean = false;
  public isRecordFound: boolean = false;
  public pageNo = 0;
  public pageSize = 20;
  public currentPage = 0;
  public totalSize = 0;
  public array: any;
  public isFilterDone = false;
  public sortedTemplateList: Template[] = [];
  public pageTypeList: any[] = [];
  public TemplateFilterForm: FormGroup;
  public filterFromDateError: boolean = false;
  public filterFromDateErrorMessage: string = "";
  public filterToDateError: boolean = false;
  public filterToDateErrorMessage: string = "";

  displayedColumns: string[] = ['name', 'pagetype', 'version', 'owner', 'publishedBy', 'date', 'status', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public userClaimsRolePrivilegeOperations: any[] = [];

  public filterDsplyName = '';
  public filterPageTypeId: number = 0;
  public filterPageStatus: string = '';
  public filterPageOwner = '';
  public filterPublishStartDate = null;
  public filterPublishEndDate = null;
  public totalRecordCount = 0;
  public sortOrder = Constants.Descending;
  public sortColumn = 'LastUpdatedDate';

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  constructor(private injector: Injector,
    private fb: FormBuilder,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private route: Router,
    private localstorageservice: LocalStorageService,
    private templateService: TemplateService) {
    this.sortedTemplateList = this.templateList.slice();
    if (localStorage.getItem('pageAddRouteparams')) {
      localStorage.removeItem('pageAddRouteparams');
    }
    if (localStorage.getItem('pageEditRouteparams')) {
      localStorage.removeItem('pageEditRouteparams');
    }
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    //this.iterator();
    this.getTemplates(null);
  }

  //Getters for Page Forms
  get filterDisplayName() {
    return this.TemplateFilterForm.get('filterDisplayName');
  }
  get filterOwner() {
    return this.TemplateFilterForm.get('filterOwner');
  }
  get filterPageType() {
    return this.TemplateFilterForm.get('filterPageType');
  }
  get filterStatus() {
    return this.TemplateFilterForm.get('filterStatus');
  }
  get filterPublishedOnFromDate() {
    return this.TemplateFilterForm.get('filterPublishedOnFromDate');
  }
  get filterPublishedOnToDate() {
    return this.TemplateFilterForm.get('filterPublishedOnToDate');
  }
  //End getter methods
  public DataFormat;
  ngOnInit() {

    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    if (userClaimsDetail) {
      this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    }
    else {
      this.localstorageservice.removeLocalStorageData();
      this.route.navigate(['login']);
    }

    this.DataFormat = localStorage.getItem('DateFormat');
    this.getTemplates(null);
    this.TemplateFilterForm = this.fb.group({
      filterDisplayName: [null],
      filterOwner: [null],
      filterStatus: [null],
      filterPageType: [null],
      filterPublishedOnFromDate: [null],
      filterPublishedOnToDate: [null],
    });
  }

  sortData(sort: MatSort) {
    const data = this.templateList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedTemplateList = data;
      return;
    }

    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    } else {
      this.sortOrder = Constants.Descending;
    }

    switch (sort.active) {
      case 'name': this.sortColumn = "DisplayName"; break;
      case 'status': this.sortColumn = "Status"; break;
      case 'pagetype': this.sortColumn = "Name"; break;
      case 'owner': this.sortColumn = "PageOwnerName"; break;
      case 'publishedBy': this.sortColumn = "PublishedByName"; break;
      case 'version': this.sortColumn = "Version"; break;
      case 'date': this.sortColumn = "PublishedOn"; break;
      default: this.sortColumn = "LastUpdatedDate"; break;
    }

    let searchParameter: any = {};
    searchParameter.IsActive = true;
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    this.getTemplates(searchParameter);
  }

  async getTemplates(searchParameter) {
    let templateService = this.injector.get(TemplateService);
    if (searchParameter == null) {
      searchParameter = {};
      searchParameter.IsActive = true;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = this.sortColumn;
      searchParameter.SortParameter.SortOrder = this.sortOrder;
      searchParameter.SearchMode = Constants.Contains;
    }
    if (this.filterDsplyName != null && this.filterDsplyName != '') {
      searchParameter.DisplayName = this.filterDsplyName.trim();
    }
    if (this.filterPageOwner != null && this.filterPageOwner != '') {
      searchParameter.PageOwner = this.filterPageOwner.trim();
    }
    if (this.filterPageTypeId != 0) {
      searchParameter.PageTypeId = this.filterPageTypeId;
    }
    if (this.filterPageStatus != null && this.filterPageStatus != '') {
      searchParameter.Status = this.filterPageStatus;
    }
    if (this.filterPublishStartDate != null && this.filterPublishStartDate != '') {
      searchParameter.StartDate = new Date(this.filterPublishStartDate.setHours(0, 0, 0));
      //searchParameter.SortParameter.SortColumn = 'PublishedOn';
    }
    if (this.filterPublishEndDate != null && this.filterPublishEndDate != '') {
      searchParameter.EndDate = new Date(this.filterPublishEndDate.setHours(23, 59, 59));
      //searchParameter.SortParameter.SortColumn = 'PublishedOn';
    }
    var response = await templateService.getTemplates(searchParameter);
    this.templateList = response.templateList;
    this.totalRecordCount = response.RecordCount;
    if (this.templateList.length == 0 && this.isFilterDone == true) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.resetPageFilterForm();
          this.getTemplates(null);
        }
      });
    }
    if (this.pageTypeList.length == 0) {
      this.getPageTypes();
    }
    this.dataSource = new MatTableDataSource<Template>(this.templateList);
    this.dataSource.sort = this.sort;
    this.array = this.templateList;
    this.totalSize = this.totalRecordCount;
    //this.iterator();
  }

  async getPageTypes() {
    let templateService = this.injector.get(TemplateService);
    this.pageTypeList = await templateService.getPageTypes();
    //if (this.pageTypeList.length == 0) {
    //  let message = ErrorMessageConstants.getNoRecordFoundMessage;
    //  this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
    //    if (data == true) {
    //      //this.getPageTypes();
    //    }
    //  });
    //}
  }

  validateFilterDate(): boolean {
    if (this.TemplateFilterForm.value.filterPublishedOnFromDate != null && this.TemplateFilterForm.value.filterPublishedOnFromDate != '' &&
      this.TemplateFilterForm.value.filterPublishedOnToDate != null && this.TemplateFilterForm.value.filterPublishedOnToDate != '') {
      let startDate = this.TemplateFilterForm.value.filterPublishedOnFromDate;
      let toDate = this.TemplateFilterForm.value.filterPublishedOnToDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        return false;
      }
    }
    return true;
  }

  onPublishedFilterDateChange(event) {
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
    let currentDte = new Date();
    if (this.TemplateFilterForm.value.filterPublishedOnFromDate != null && this.TemplateFilterForm.value.filterPublishedOnFromDate != '') {
      let startDate = this.TemplateFilterForm.value.filterPublishedOnFromDate;
      if (startDate.getTime() > currentDte.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanCurrentDateMessage;
      }
    }
    if (this.TemplateFilterForm.value.filterPublishedOnToDate != null && this.TemplateFilterForm.value.filterPublishedOnToDate != '') {
      let toDate = this.TemplateFilterForm.value.filterPublishedOnToDate;
      if (toDate.getTime() > currentDte.getTime()) {
        this.filterToDateError = true;
        this.filterToDateErrorMessage = ErrorMessageConstants.getEndDateLessThanCurrentDateMessage;
      }
    }
    if (this.TemplateFilterForm.value.filterPublishedOnFromDate != null && this.TemplateFilterForm.value.filterPublishedOnFromDate != '' &&
      this.TemplateFilterForm.value.filterPublishedOnToDate != null && this.TemplateFilterForm.value.filterPublishedOnToDate != '') {
      let startDate = this.TemplateFilterForm.value.filterPublishedOnFromDate;
      let toDate = this.TemplateFilterForm.value.filterPublishedOnToDate;
      if (startDate.getTime() > toDate.getTime()) {
        this.filterFromDateError = true;
        this.filterFromDateErrorMessage = ErrorMessageConstants.getStartDateLessThanEndDateMessage;
      }
    }
  }

  //This method has been used for fetching search records
  searchTemplateRecordFilter(searchType) {
    this.filterFromDateError = false;
    this.isFilterDone = true;
    if (searchType == 'reset') {
      this.resetPageFilterForm();
      this.getTemplates(null);
      this.isFilter = !this.isFilter;
    }
    else {
      if (this.validateFilterDate()) {
        let searchParameter: any = {};
        searchParameter.PagingParameter = {};
        searchParameter.PagingParameter.PageIndex = 1;
        searchParameter.PagingParameter.PageSize = this.pageSize;
        searchParameter.SortParameter = {};
        searchParameter.SortParameter.SortColumn = this.sortColumn;
        searchParameter.SortParameter.SortOrder = this.sortOrder;
        searchParameter.SearchMode = Constants.Contains;

        if (this.TemplateFilterForm.value.filterDisplayName != null && this.TemplateFilterForm.value.filterDisplayName != '') {
          this.filterDsplyName = this.TemplateFilterForm.value.filterDisplayName.trim();
          searchParameter.DisplayName = this.TemplateFilterForm.value.filterDisplayName.trim();
        }
        if (this.TemplateFilterForm.value.filterOwner != null && this.TemplateFilterForm.value.filterOwner != '') {
          this.filterPageOwner = this.TemplateFilterForm.value.filterOwner.trim();
          searchParameter.PageOwner = this.TemplateFilterForm.value.filterOwner.trim();
        }
        if (this.filterPageTypeId != 0) {
          searchParameter.PageTypeId = this.filterPageTypeId;
        }
        if (this.TemplateFilterForm.value.filterStatus != null && this.TemplateFilterForm.value.filterStatus != 0) {
          this.filterPageStatus = this.TemplateFilterForm.value.filterStatus;
          searchParameter.Status = this.TemplateFilterForm.value.filterStatus;
        }
        if (this.TemplateFilterForm.value.filterPublishedOnFromDate != null && this.TemplateFilterForm.value.filterPublishedOnFromDate != '') {
          this.filterPublishStartDate = this.TemplateFilterForm.value.filterPublishedOnFromDate;
          searchParameter.StartDate = new Date(this.TemplateFilterForm.value.filterPublishedOnFromDate.setHours(0, 0, 0));
          //searchParameter.SortParameter.SortColumn = 'PublishedOn';
        }
        if (this.TemplateFilterForm.value.filterPublishedOnToDate != null && this.TemplateFilterForm.value.filterPublishedOnToDate != '') {
          this.filterPublishEndDate = this.TemplateFilterForm.value.filterPublishedOnToDate;
          searchParameter.EndDate = new Date(this.TemplateFilterForm.value.filterPublishedOnToDate.setHours(23, 59, 59));
          //searchParameter.SortParameter.SortColumn = 'PublishedOn';
        }

        //console.log(searchParameter);
        this.currentPage = 0;
        this.getTemplates(searchParameter);
        this.isFilter = !this.isFilter;
      }
    }
  }

  resetPageFilterForm() {
    this.TemplateFilterForm.patchValue({
      filterDisplayName: null,
      filterOwner: null,
      filterPageType: 0,
      filterStatus: 0,
      filterPublishedOnFromDate: null,
      filterPublishedOnToDate: null
    });

    this.currentPage = 0;
    this.filterDsplyName = '';
    this.filterPageOwner = '';
    this.filterPageStatus = '';
    this.filterPageTypeId = 0;
    this.filterPublishStartDate = null;
    this.filterPublishEndDate = null;
    this.filterFromDateError = false;
    this.filterToDateError = false;
    this.filterFromDateErrorMessage = "";
    this.filterToDateErrorMessage = "";
  }

  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  public onPageTypeSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.filterPageTypeId = 0;
    }
    else {
      this.filterPageTypeId = Number(value);
    }
  }

  navigationTodashboardDesigner(template: Template) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "PageName": template.DisplayName,
          "PageIdentifier": template.Identifier,
        }
      }
    }
    localStorage.setItem("pageDesignViewRouteparams", JSON.stringify(queryParams))
    this.route.navigate(['../dashboardDesignerView']);
  }

  async DeletePage(template: Template) {
    let message = "Are you sure, you want to delete this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let pageData = [{
          "Identifier": template.Identifier,
        }];

        let resultFlag = await this.templateService.deletePage(pageData);
        if (resultFlag) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.resetPageFilterForm();
          this.getTemplates(null);
        }
      }
    });
  }

  async PublishPage(template: Template) {
    let message = "Are you sure, you want to publish this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let pageData = [{
          "Identifier": template.Identifier,
        }];
        let resultFlag = await this.templateService.publishPage(pageData);
        if (resultFlag) {
          let messageString = Constants.PagePublishedSuccessfullyMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.resetPageFilterForm();
          this.getTemplates(null);
        }
      }
    });
  }

  async ClonePage(template: Template) {
    let message = "Are you sure, you want to clone this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let pageData = [{
          "Identifier": template.Identifier,
        }];
        let resultFlag = await this.templateService.clonePage(pageData);
        if (resultFlag) {
          let messageString = Constants.PageCloneSuccessfullyMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.resetPageFilterForm();
          this.getTemplates(null);
        }
      }
    });
  }

  async PreviewPage(template: Template) {
    let pageData = [{
      "Identifier": template.Identifier,
    }];
    let resultHtmlString = await this.templateService.previewPage(pageData);
    if (resultHtmlString != '') {
      let previewService = this.injector.get(PreviewDialogService);
      previewService.openPreviewDialogBox(resultHtmlString);
    }
  }

  navigationToEditPage(template: Template) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "PageName": template.DisplayName,
          "PageIdentifier": template.Identifier,
          "PageTypeId": template.PageTypeId,
          "BackgroundImageAssetLibraryId": template.BackgroundImageAssetLibraryId != null ? template.BackgroundImageAssetLibraryId : 0,
          "BackgroundImageAssetId": template.BackgroundImageAssetId != null ? template.BackgroundImageAssetId : 0,
          "BackgroundImageURL": template.BackgroundImageURL != null ? template.BackgroundImageURL : '',
          "pageEditModeOn": true
        }
      }
    }
    localStorage.setItem("pageEditRouteparams", JSON.stringify(queryParams))
    this.route.navigate(['pages', 'Edit']);
  }

  navigationToDashboardDesignerEdit(template: Template) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "PageName": template.DisplayName,
          "PageIdentifier": template.Identifier,
          "pageEditModeOn": true
        }
      }
    }
    localStorage.setItem("pageDesignEditRouteparams", JSON.stringify(queryParams))
    this.route.navigate(['../dashboardDesigner']);
  }

}
