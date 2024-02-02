import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { HttpClient, HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { UserService } from '../user.service';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { CellRenderService } from 'src/app/shared/services/cellsrenderer';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { Constants } from 'src/app/shared/constants/constants';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { PaginationInstance } from 'src/app/shared/modules/pagination/pagination.module';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { ResourceService } from 'src/app/shared/services/resource.service';
import { User } from '../user';
import { LoginService } from '../../../login/login.service';
@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})

export class ListComponent implements OnInit {

  userFormGroup: FormGroup;
  public isFilter: boolean = false;

  public userList;
  public userLists = [];
  public roleList = [{ "Name": "Select Role", "Identifier": 0 }];
  public dataAdapter: any = [];
  public columns = [];
  public actionCellsrenderer: any;
  public createWidget: boolean;
  public message;
  public searchForm: FormGroup;
  public searchedUserList;
  public status;
  public displayRecords;
  public isLoaderActive: boolean = false;
  public params;
  public UserIdentifier;
  public UserName;
  public preferredLanguage;
  public userListResources = {}
  public ResourceLoadingFailedMsg = Constants.ResourceLoadingFailedMsg;
  public Locale;
  public sectionStr;
  public userClaimsRolePrivilegeOperations;
  public roleListsArr: any = [];
  public roleListSource: any[] = []
  public lockSlider: boolean = false;
  public statusSlider: boolean = false;
  public userImage;
  public isRecordFound: boolean;
  public organizationUnitName;
  public emailId;
  public designation;
  public roles;
  public isLocked: boolean
  public isActive: boolean
  public designationList = [];
  public designationLists: any = [];
  public designationListSource: any = [];
  public languageList = [];
  public languageLists: any = [];
  public languageSource: any[] = []
  public ouList: any[] = []
  public ouName;
  public isFilterDone = false;
  displayedColumns: string[] = ['name', 'email', 'mobileno', 'role', 'active', 'lock', 'actions'];
  dataSource = new MatTableDataSource<any>();
  public sortOrder = Constants.Ascending;
  public sortColumn = 'FirstName';

  public sortedUserList = [];

  public lockStatusArray: any[] = [
    {
      'Identifier': 0,
      'Name': 'Both'
    },
    {
      'Identifier': 1,
      'Name': 'Locked'
    },
    {
      'Identifier': 2,
      'Name': 'Unlocked'
    }

  ];

  public activeStatusArray: any[] = [

    {
      'Identifier': 0,
      'Name': 'Both'
    },
    {
      'Identifier': 1,
      'Name': 'Active'
    },
    {
      'Identifier': 2,
      'Name': 'Inactive'
    }
  ];

  public array: any;

  public pageSize = 10;
  public currentPage = 0;
  public totalSize = 0;
  public previousPageLabel: string;
  public totalRecordCount = 0;
  public loggedInUserIdentifier;
  public profileImage;
  public UserFilter: any = {
    FirstName: null,
    LastName: null,
    Code: null,
    EmailAddress: null,
    MobileNumber: null,
    OrganisationUnitIdentifier: null,
    RoleIdentifier: null,
    DesignationIdentifier: null,
    PreferedLanguageIdentifier: null,
    LockStatus: null,
    ActivationStatus: true,
  };

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

 
  constructor(private http: HttpClient,
    private formBuilder: FormBuilder,
    private service: UserService,
    private router: Router,
    private route: ActivatedRoute,
    private actionCellWindow: CellRenderService,
    private _dialogService: DialogService,
    private localstorageservice: LocalStorageService,
    private fb: FormBuilder,
    private spinner: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private injector: Injector,
    private loginService: LoginService,
  ) {
    //remove localstorage item.
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/user')) {

        } else {
          localStorage.removeItem("userRouteparams")
        }
      }
    });
    //getting localstorage item
    if (localStorage.getItem("userRouteparams")) {
      this.params = JSON.parse(localStorage.getItem('userRouteparams'));
      this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier;
      this.UserFilter.FirstName = this.params.Routeparams.filteredparams.FirstName;
      this.UserFilter.LastName = this.params.Routeparams.filteredparams.LastName;
      this.UserFilter.Code = this.params.Routeparams.filteredparams.Code;
      this.UserFilter.EmailAddress = this.params.Routeparams.filteredparams.EmailAddress;
      this.UserFilter.OrganisationUnitIdentifier = this.params.Routeparams.filteredparams.OrganisationUnitIdentifier;
      this.UserFilter.MobileNumber = this.params.Routeparams.filteredparams.MobileNumber;
      this.UserFilter.DesignationIdentifier = this.params.Routeparams.filteredparams.DesignationIdentifier;
      this.UserFilter.RoleIdentifier = this.params.Routeparams.filteredparams.RoleIdentifier;
      this.UserFilter.LockStatus = this.params.Routeparams.filteredparams.LockStatus;
      this.UserFilter.ActivationStatus = this.params.Routeparams.filteredparams.ActivationStatus;;
    }

    this.sortedUserList = this.userLists.slice();
  }

  sortData(sort: MatSort) {
    const data = this.roleList.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedUserList = data;
      return;
    }
    if (sort.direction == 'asc') {
      this.sortOrder = Constants.Ascending;
    }
    else {
      this.sortOrder = Constants.Descending;
    }
    if (sort.active == 'name') {
      this.sortColumn = 'FirstName';
    }
    else if (sort.active == 'email') {
      this.sortColumn = 'EmailAddress';
    }
    else if (sort.active == 'mobileno') {
     this. sortColumn = 'ContactNumber';
    }
    else if (sort.active == 'active') {
      this.sortColumn = 'IsActive';
    }
    else if (sort.active == 'lock') {
      this.sortColumn = 'IsLocked';
    }
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    if (this.userFormGroup.value.FirstName != null) {
      searchParameter.FirstName = this.UserFilter.FirstName.trim();
    }
    if (this.userFormGroup.value.EmailAddress != null) {
      searchParameter.EmailAddress = this.UserFilter.EmailAddress.trim();
    }
    if (this.UserFilter.RoleIdentifier != null) {
      searchParameter.RoleIdentifier = this.UserFilter.RoleIdentifier;
    }
    searchParameter.LockStatus = this.UserFilter.LockStatus;
    searchParameter.ActivationStatus = this.UserFilter.ActivationStatus;
    this.getUserdetail(searchParameter);
  }

  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage+1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.UserName;
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    if (this.userFormGroup.value.FirstName != null) {
      searchParameter.FirstName = this.UserFilter.FirstName.trim();
    }
    if (this.userFormGroup.value.EmailAddress != null) {
      searchParameter.EmailAddress = this.UserFilter.EmailAddress.trim();
    }
    if (this.UserFilter.RoleIdentifier != null) {
      searchParameter.RoleIdentifier = this.UserFilter.RoleIdentifier;
    }
    searchParameter.LockStatus = this.UserFilter.LockStatus;
    searchParameter.ActivationStatus = this.UserFilter.ActivationStatus;
    this.getUserdetail(searchParameter);
  }

  private iterator() {
    const end = (this.currentPage + 1) * this.pageSize;
    const start = this.currentPage * this.pageSize;
    const part = this.array.slice(start, end);
    this.dataSource = part;
    this.dataSource.sort = this.sort;
  }


  ngOnInit() {
    // this.getUserdetail();
    this.userFormGroup = this.formBuilder.group({
      UserRole: [0, Validators.compose([])],
      FirstName: ['', Validators.compose([])],
      EmailAddress: ['', Validators.compose([])],
      UserActiveStatus: [1, Validators.compose([])],
      UserLockStatus: [0, Validators.compose([])],
    })
    this.getRoles();
    var userClaimsDetail = JSON.parse(localStorage.getItem('userClaims'));
    this.userClaimsRolePrivilegeOperations = userClaimsDetail.Privileges;
    this.loggedInUserIdentifier = userClaimsDetail.UserIdentifier;
    this.fetchUserRecord();
  }

  //Get api for fetching User details--
  async getUserdetail(searchParameter) {
    if (searchParameter == null) {
      let newsearchParameter: any = {};
      newsearchParameter.PagingParameter = {};
      newsearchParameter.PagingParameter.PageIndex = this.currentPage + 1;
      newsearchParameter.PagingParameter.PageSize = this.pageSize;
      newsearchParameter.SortParameter = {};
      newsearchParameter.SortParameter.SortColumn = Constants.UserName;
      newsearchParameter.SortParameter.SortOrder = Constants.Ascending;
      newsearchParameter.SearchMode = Constants.Contains;
      newsearchParameter.ActivationStatus = true;
      searchParameter = newsearchParameter;
    }

    var response = await this.service.getUser(searchParameter);
    this.userLists = response.usersList;
    this.totalRecordCount = response.RecordCount;

    if (this.userLists.length == 0 && this.isFilterDone == true) {
      let message = "No record found";//this.roleListResources['lblNoRecord'] == undefined ? this.ResourceLoadingFailedMsg : this.roleListResources['lblNoRecord']
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getAllUSer();
        }
      });
    }
    this.userLists.forEach(el => {
      if (el.Image != '' && el.Image != null) {
        el.Image = el.Image;
      }

    })

    if (this.userLists.length == 0 && this.isFilterDone == true) {
      let message = "User not found"

    }
    this.dataSource = new MatTableDataSource<User>(this.userLists);
    this.dataSource.sort = this.sort;
    this.array = this.userLists;
    this.totalSize = this.totalRecordCount;

    if (this.userLists.length > 0) {
    }
    else {
      this.status = "No Records Found";
    }
  }

  fetchUserRecord() {
    this.params = JSON.parse(localStorage.getItem('userRouteparams'));
    if (localStorage.getItem('userRouteparams')) {
      this.UserIdentifier = this.params.Routeparams.passingparams.UserIdentifier;
      this.UserFilter.FirstName = this.params.Routeparams.filteredparams.FirstName;
      this.UserFilter.LastName = this.params.Routeparams.filteredparams.LastName;
      this.UserFilter.Code = this.params.Routeparams.filteredparams.Code;
      this.UserFilter.EmailAddress = this.params.Routeparams.filteredparams.EmailAddress;
      this.UserFilter.OrganisationUnitIdentifier = this.params.Routeparams.filteredparams.OrganisationUnitIdentifier;
      this.UserFilter.MobileNumber = this.params.Routeparams.filteredparams.MobileNumber;
      this.UserFilter.DesignationIdentifier = this.params.Routeparams.filteredparams.DesignationIdentifier;
      this.UserFilter.RoleIdentifier = this.params.Routeparams.filteredparams.RoleIdentifier;
      this.UserFilter.PreferedLanguageIdentifier = this.params.Routeparams.filteredparams.PreferedLanguageIdentifier;
      this.UserFilter.LockStatus = this.params.Routeparams.filteredparams.LockStatus;
      this.UserFilter.ActivationStatus = this.params.Routeparams.filteredparams.ActivationStatus;
    }
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = this.currentPage + 1;
    searchParameter.PagingParameter.PageSize = this.pageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = this.sortColumn;
    searchParameter.SortParameter.SortOrder = this.sortOrder;
    searchParameter.SearchMode = Constants.Contains;
    searchParameter.GetResources = true;
    if (this.UserFilter.FirstName != null) {
      searchParameter.FirstName = this.UserFilter.FirstName;
    }
    if (this.UserFilter.LastName != null) {
      searchParameter.LastName = this.UserFilter.LastName;
    }
    if (this.UserFilter.Code != null) {
      searchParameter.Code = this.UserFilter.Code;
    }
    if (this.UserFilter.EmailAddress != null) {
      searchParameter.EmailAddress = this.UserFilter.EmailAddress;
    }
    if (this.UserFilter.MobileNumber != null) {
      searchParameter.MobileNumber = this.UserFilter.MobileNumber;
    }
    if (this.UserFilter.OrganisationUnitIdentifier != null) {
      searchParameter.OrganisationUnitIdentifier = this.UserFilter.OrganisationUnitIdentifier;
    }
    if (this.UserFilter.DesignationIdentifier != null) {
      searchParameter.DesignationIdentifier = this.UserFilter.DesignationIdentifier;
    }
    if (this.UserFilter.PreferedLanguageIdentifier != null) {
      searchParameter.PreferedLanguageIdentifier = this.UserFilter.PreferedLanguageIdentifier;
    }
    if (this.UserFilter.RoleIdentifier != null) {
      searchParameter.RoleIdentifier = this.UserFilter.RoleIdentifier;
    }
    searchParameter.LockStatus = this.UserFilter.LockStatus;
    searchParameter.ActivationStatus = this.UserFilter.ActivationStatus;
    this.getUserdetail(searchParameter);
  }

  async getRoles() {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    //searchParameter.GetPrivileges = true;
    //this.spinner.start();
    var copy = await this.loginService.getRoles(searchParameter);
    //this.spinner.stop();
    copy.forEach(role => {
      this.roleList.push(role);
    })
  }

  //Function to navigate to view page of perticular user detail--
  viewUser(user) {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "UserIdentifier": user.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "FirstName": this.UserFilter.FirstName != null ? this.UserFilter.FirstName : null,
          "LastName": this.UserFilter.LastName != null ? this.UserFilter.LastName : null,
          "Code": this.UserFilter.Code != null ? this.UserFilter.Code : null,
          "EmailAddress": this.UserFilter.EmailAddress != null ? this.UserFilter.EmailAddress : null,
          "MobileNumber": this.UserFilter.MobileNumber != null ? this.UserFilter.MobileNumber : null,
          "OrganisationUnitIdentifier": this.UserFilter.OrganisationUnitIdentifier != null ? this.UserFilter.OrganisationUnitIdentifier : null,
          "DesignationIdentifier": this.UserFilter.DesignationIdentifier != null ? this.UserFilter.DesignationIdentifier : null,
          "PreferedLanguageIdentifier": this.UserFilter.PreferedLanguageIdentifier != null ? this.UserFilter.PreferedLanguageIdentifier : null,
          "RoleIdentifier": this.UserFilter.RoleIdentifier != null ? this.UserFilter.RoleIdentifier : null,
          "LockStatus": this.UserFilter.LockStatus,
          "ActivationStatus": this.UserFilter.ActivationStatus,
        }
      }
    }
    localStorage.setItem("userRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['user', 'userView']);
  }

  //Function to edit perticular user--
  editUser(user) {

    let queryParams = {
      Routeparams: {
        passingparams: {
          "UserIdentifier": user.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "FirstName": this.UserFilter.FirstName != null ? this.UserFilter.FirstName : null,
          "LastName": this.UserFilter.LastName != null ? this.UserFilter.LastName : null,
          "EmailAddress": this.UserFilter.EmailAddress != null ? this.UserFilter.EmailAddress : null,
          "MobileNumber": this.UserFilter.MobileNumber != null ? this.UserFilter.MobileNumber : null,
          "RoleIdentifier": this.UserFilter.RoleIdentifier != null ? this.UserFilter.RoleIdentifier : null,
          "LockStatus": this.UserFilter.LockStatus != null ? this.UserFilter.LockStatus : null,
          "ActivationStatus": this.UserFilter.ActivationStatus != null ? this.UserFilter.ActivationStatus : null,
        }
      }
    }
    localStorage.setItem("userRouteparams", JSON.stringify(queryParams))
    this.router.navigate(['user', 'userEdit']);
  }

  //function written to delete user--
  deleteUser(user: User) {
    let message = "Are you sure you want to delete this record?";
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {

        let isDeleted = await this.service.deleteUser(user.Identifier);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this.fetchUserRecord();
        }
      }
    });
  }

  //function written to unlock user--
  unLockUser(user: User) {
    if (user.IsLocked) {
      let message = "Do you want to unlock user??"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let isDeleted = await this.service.unlockUser(user.Identifier);
          if (isDeleted) {
            let messageString = Constants.recordUnlockedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchUserRecord();
          }
        }
      });
    }
    else {
      let message = "Do you want to lock user??"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let isDeleted = await this.service.userlockUrl(user.Identifier);
          if (isDeleted) {
            let messageString = Constants.recordlockedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchUserRecord();
          }
        }
      });
    }

  }

  activeDeactiveUser(user: User) {
    let message;
    if (user.IsActive) {
      message = "Do you really want to deactivate user?"
      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let isDeleted = await this.service.deactivate(user.Identifier);
          if (isDeleted) {
            let messageString = "User deactivated successfully";
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchUserRecord();
          }
        }
      });
    }
    else {
      message = "Do you really want to activate user?"

      this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
        if (isConfirmed) {
          this.isLoaderActive = true;
          let isDeleted = await this.service.activate(user.Identifier);
          if (isDeleted) {
            let messageString = "User activated successfully";
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.fetchUserRecord();
          }
        }
      });
    }

  }

  //this method helps to navigate to add
  navigateToAddUser() {
    this.router.navigate(['user', 'userAdd']);
  }

  //this method helps to reset user password
  resetPassword(tenantuser) {
    let message = 'Are you sure, you want to reset password for this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let data = {
          "EmailAddress": tenantuser.EmailAddress
        };
        let result = await this.service.sendPassword(data);
        if (result) {
          let messageString = Constants.sentPasswordMailMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
        }
      }
    });
  }

  public onRoleSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.UserFilter.RoleIdentifier = null;
    }
    else {
      this.UserFilter.RoleIdentifier = Number(value);

    }
  }

  public onLockStatusSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.UserFilter.LockStatus = null;
    }
    else if (value == "1") {
      this.UserFilter.LockStatus = true;
    }
    else if (value == "2") {
      this.UserFilter.LockStatus = false;


    }
  }

  public onActiveStatusSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.UserFilter.ActivationStatus = null;
    }
    else if (value == "1") {
      this.UserFilter.ActivationStatus = true;


    }
    else if (value == "2") {
      this.UserFilter.ActivationStatus = false;


    }
  }

  //User filter function--
  getAllUSer() {
    this.isFilterDone = true;
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = 1;
    searchParameter.PagingParameter.PageSize = 5;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.UserName;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    this.UserFilter = {
      FirstName: null,
      LastName: null,
      Code: null,
      EmailAddress: null,
      MobileNumber: null,
      OrganisationUnitIdentifier: null,
      RoleIdentifier: null,
      LockStatus: null,
      ActivationStatus: null,
    };

    this.getUserdetail(searchParameter);
    this.userFormGroup.controls['UserRole'].setValue(0);
    this.userFormGroup.controls['UserLockStatus'].setValue(0);
    this.userFormGroup.controls['UserActiveStatus'].setValue(0);
  }

  //User filter function--
  filterSetUp(searchType) {
    this.isFilterDone = true;
    if (searchType == 'Reset') {
      localStorage.removeItem("userRouteparams");
      this.UserFilter = {
        FirstName: null,
        LastName: null,
        Code: null,
        EmailAddress: null,
        MobileNumber: null,
        OrganisationUnitIdentifier: null,
        RoleIdentifier: null,
        LockStatus: null,
        ActivationStatus: null,
      };
      this.userFormGroup.controls['UserRole'].setValue(0);
      this.userFormGroup.controls['UserLockStatus'].setValue(0);
      this.userFormGroup.controls['UserActiveStatus'].setValue(0);
      this.isFilter = !this.isFilter;
      this.fetchUserRecord();
      this.userLists = [];
      this.searchedUserList = [];
    }
    else {
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = 1;
      searchParameter.PagingParameter.PageSize = this.pageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.UserName;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      if (this.userFormGroup.value.FirstName != null) {
        searchParameter.FirstName = this.UserFilter.FirstName.trim();
      }
      if (this.userFormGroup.value.EmailAddress != null) {
        searchParameter.EmailAddress = this.UserFilter.EmailAddress.trim();
      }
      if (this.UserFilter.RoleIdentifier != null) {
        searchParameter.RoleIdentifier = this.UserFilter.RoleIdentifier;
      }
      searchParameter.LockStatus = this.UserFilter.LockStatus;
      searchParameter.ActivationStatus = this.UserFilter.ActivationStatus;
      this.currentPage = 0;
      this.getUserdetail(searchParameter);
      this.isFilter = !this.isFilter;
    }
  }

  //Function to close the filter popup--
  closeFilter() {
    this.isFilter = !this.isFilter;
  }

  activationEventCheck(event) {
    if (event.checked) {
      this.UserFilter.ActivationStatus = true
    }
    else {
      this.UserFilter.ActivationStatus = false;
    }
  }

  lockEventCheck(event) {
    if (event.checked) {
      this.UserFilter.LockStatus = true;
    }
    else {
      this.UserFilter.LockStatus = false;
    }
  }
}

function compareStr(a: string, b: string, isAsc: boolean) {
  return (a.toLowerCase() < b.toLowerCase() ? -1 : 1) * (isAsc ? 1 : -1);
}

function compareNumber(a: number, b: number, isAsc: boolean) {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
