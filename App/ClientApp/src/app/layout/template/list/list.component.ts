import { Component, OnInit, Injector, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, NavigationEnd } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ResourceService } from 'src/app/shared/services/resource.service';
import { MatPaginator } from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TemplateService } from '../template.service';
import { Template } from '../template';

export interface ListElement {
    name: string;
    version: string;
    owner: string;
    date: string;
    status: string;
    pagetype: string;
}

@Component({
    selector: 'app-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    constructor(private injector: Injector,
        private fb: FormBuilder,
        private uiLoader: NgxUiLoaderService,
        private _messageDialogService: MessageDialogService,
        private route: Router,
        private localstorageservice: LocalStorageService,
        private templateService: TemplateService) 
        {
            this.sortedTemplateList = this.templateList.slice();
        }

    //public variables
    public isFilter: boolean = false;
    public templateList: Template[] = [];
    public isLoaderActive: boolean = false;
    public isRecordFound: boolean = false;
    public pageNo = 0;
    public pageSize = 5;
    public currentPage = 0;
    public totalSize = 0;
    public array: any;
    public isFilterDone = false;
    public sortedTemplateList : Template[] = [];
    public pageTypeList: any[] = [];
    public TemplateFilterForm: FormGroup;

    displayedColumns: string[] = ['name','pagetype', 'version', 'owner', 'date', 'status', 'actions'];
    dataSource = new MatTableDataSource<any>();

    @ViewChild(MatSort, { static: true }) sort: MatSort;
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

    public handlePage(e: any) {
        this.currentPage = e.pageIndex;
        this.pageSize = e.pageSize;
        this.iterator();
    }

    private iterator() {
        const end = (this.currentPage + 1) * this.pageSize;
        const start = this.currentPage * this.pageSize;
        const part = this.array.slice(start, end);
        this.dataSource = part;
        this.dataSource.sort = this.sort;
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

    ngOnInit() {
        this.getTemplates(null);
        this.getPageTypes(null);

        this.TemplateFilterForm = this.fb.group(
            {
                filterDisplayName: [null],
                filterOwner: [null],
                filterStatus: [null],
                filterPageType: [null],
                filterPublishedOnFromDate: [null],
                filterPublishedOnToDate: [null],
            }
        );
    }
    
    ngAfterViewInit() {
        //this.dataSource.paginator = this.paginator;
    }

    sortData(sort: MatSort) {
        const data = this.templateList.slice();
        if (!sort.active || sort.direction === '') {
            this.sortedTemplateList = data;
            return;
        }

        this.sortedTemplateList = data.sort((a, b) => {
            const isAsc = sort.direction === 'asc';
            switch (sort.active) {
                case 'name': return compareStr(a.DisplayName, b.DisplayName, isAsc);
                case 'status': return compareStr(a.Status, b.Status, isAsc);
                case 'pagetype': return compareStr(a.PageTypeName, b.PageTypeName, isAsc);
                case 'owner': return compareStr(a.PageOwnerName, b.PageOwnerName, isAsc);
                //case 'date': return compareDate(a.PublishedOn, b.PublishedOn, isAsc);
                default: return 0;
            }
        });
        this.dataSource = new MatTableDataSource<Template>(this.sortedTemplateList);
        this.dataSource.sort = this.sort;
        this.array = this.sortedTemplateList;
        this.totalSize = this.array.length;
        this.iterator();
    }

    async getTemplates(searchParameter) {
        let templateService = this.injector.get(TemplateService);
        if (searchParameter == null) {
            searchParameter = {};
            searchParameter.IsActive = true;
            searchParameter.PagingParameter = {};
            searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
            searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
            searchParameter.SortParameter = {};
            searchParameter.SortParameter.SortColumn = 'DisplayName';
            searchParameter.SortParameter.SortOrder = Constants.Ascending;
            searchParameter.SearchMode = Constants.Contains;
        }
        this.templateList = await templateService.getTemplates(searchParameter);
        if (this.templateList.length == 0 && this.isFilterDone == true){
            let message = "NO record found";
            this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
                if (data == true) {
                    //this.resetRoleFilterForm();
                    this.getTemplates(null);
                }
            });
        }
        this.dataSource = new MatTableDataSource<Template>(this.templateList);
        this.dataSource.sort = this.sort;
        this.array = this.templateList;
        this.totalSize = this.array.length;
        this.iterator();
    }

    async getPageTypes(searchParameter) {
        this.pageTypeList = [{ "Name": "Select Role", "Id": 0 }, {"Id": 1, "Name": "Home"}, {"Id": 2, "Name": "Saving Account"}, {"Id": 3, "Name": "Current Account"} ];
    }

    //This method has been used for fetching search records
    searchTemplateRecordFilter(searchType) {
        this.isFilterDone = true;
        if (searchType == 'reset') {
            this.resetPageFilterForm();
            this.getTemplates(null);
            this.isFilter = !this.isFilter;
        }
        else {
            let searchParameter: any = {};
            searchParameter.PagingParameter = {};
            searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
            searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
            searchParameter.SortParameter = {};
            searchParameter.SortParameter.SortColumn = 'DisplayName';
            searchParameter.SortParameter.SortOrder = Constants.Ascending;
            searchParameter.SearchMode = Constants.Contains;
            searchParameter.DisplayName = this.TemplateFilterForm.value.filterDisplayName != null ? this.TemplateFilterForm.value.filterDisplayName : "";
            //searchParameter.IsActive = this.roleFilterForm.value.DeactivateRole != null ? !this.roleFilterForm.value.DeactivateRole: true;
            this.getTemplates(searchParameter);
            this.isFilter = !this.isFilter;
        }
    }

    resetPageFilterForm(){
        this.TemplateFilterForm.patchValue({
            filterDisplayName: null,
            filterOwner: null,
            filterPageType: 0,
            filterStatus: 0,
            filterPublishedOnFromDate: null,
            filterPublishedOnToDate: null
        });
      }

    closeFilter() {
        this.isFilter = !this.isFilter;
    }

    public onPageTypeSelected(event) {
        const value = event.target.value;
        if (value == "0") {
          //this.UserFilter.RoleIdentifier = null;
        }
        else {
          //this.UserFilter.RoleIdentifier = Number(value);
        }
      }

    navigationTodashboardDesigner() {
        this.route.navigate(['../dashboardDesignerView']);
    }
}

function compareStr(a: string, b: string, isAsc: boolean) {
    return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
  }
  
  function compareDate(a: Date, b: Date, isAsc: boolean) {
    return (a.getTime() < b.getTime() ? -1 : 1) * (isAsc ? 1 : -1);
  }
