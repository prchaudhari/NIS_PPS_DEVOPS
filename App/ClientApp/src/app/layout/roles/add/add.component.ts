import { Component, OnInit, Injector, ChangeDetectorRef } from '@angular/core';
import { Location } from '@angular/common';
import { Constants } from 'src/app/shared/constants/constants';
import { FormGroup, FormBuilder, Validators, FormControl, SelectControlValueAccessor, FormArray, ValidatorFn } from '@angular/forms';
import { RoleService } from '../role.service';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {

    //variable declaration here
    public isCollapsedRoles: boolean = true;
    public isCollapsedDetails: boolean = false;
    public isCollapsedPermissions: boolean = true;
    public isLoaderActive: boolean = false;
    public isShowFilterRoles: boolean = false;
    public roleFormGroup: FormGroup;
    public permissonFilterForm: FormGroup;
    public params: any = {};
    public RoleIdentifier;
    public RoleName;
    public status;
    public EntityName: any = []
    public DisplayName: any = []
    public RolePrivilegeOperations: any = [];
    public Operation;
    public IsEnabled;
    public EntityNames
    public checked;
    public roleLists = [];
    public rolRecord = [];
    public categoriesSelected = []
    isTrade: boolean = false;
    public checkedRolePrivilages = [];
    checkAllTrades: boolean = false
    public title: string = "Add";
    public roleFormErrorObject: any = {
        showRoleNameError: false
    };
    public roleEditModeOn: boolean = false;
    //regex
    public onlyAlphabetsWithSpace = '[a-zA-Z ]*';
    public onlyAlphabetsWithSpaceQuoteHyphen = Constants.onlyAlphabetsWithSpaceQuoteHyphen;
    public rolePrivilegesList = [];
    public entityList = [];
    public roleAddEditResources = {}
    public ResourceLoadingFailedMsg = Constants.ResourceLoadingFailedMsg;
    public allPermisions: boolean = false
    public rowWisePermisions: boolean = false;
    // public IsAllRolePrevilegeSelected: boolean = false;
    public chcekBoxValue;
    public Locale;
    public trackByFn;
    public checkedRole = [];
    public data = [];
    public selectedAll: any = [];
    public IsAllRolePrevilegeSelected: any;
    public selectAll: any;
    public check: boolean = false;
    public checkedRoles = [];
    public userClaimsRolePrivilegeOperations;
    public IsAllRole: any;

    //getters of roleForm group
    get roleName() {
        return this.roleFormGroup.get('roleName');
    }

    //function to validate all fields
    validateAllFormFields(formGroup: FormGroup) {
        Object.keys(formGroup.controls).forEach(field => {
            const control = formGroup.get(field);
            if (control instanceof FormControl) {
                control.markAsTouched({ onlySelf: true });
            } else if (control instanceof FormGroup) {
                this.validateAllFormFields(control);
            }
        });
    }

    constructor(private _location: Location,
      private formbuilder: FormBuilder,
      private injector: Injector,
      private uiLoader: NgxUiLoaderService,
      private _messageDialogService: MessageDialogService,
      private router: Router,
      private localstorageservice: LocalStorageService,
      private spinner: NgxUiLoaderService,
      private roleService: RoleService ) {
          router.events.subscribe(e => {
            if (e instanceof NavigationEnd) {
                if (e.url.includes('/roles/Add')) {
                    this.roleEditModeOn = false;
                    this.allPermisions = false
                    this.IsAllRolePrevilegeSelected = false;
                    localStorage.removeItem("roleparams");
                }
            }
        });

        if (localStorage.getItem("roleparams")) {
            this.roleEditModeOn = true;
        } else {
            this.roleEditModeOn = false;
        }

        router.events.subscribe(e => {
            if (e instanceof NavigationEnd) {
                if (e.url.includes('/roles')) {
                    //set passing parameters to localstorage.
                    this.params = JSON.parse(localStorage.getItem('roleparams'));
                    if (localStorage.getItem('roleparams')) {
                        this.RoleIdentifier = this.params.Routeparams.passingparams.RoleIdentifier
                        this.RoleName = this.params.Routeparams.filteredparams.RoleName
                    }
                } else {
                    localStorage.removeItem("roleparams");
                }
            }
        });
    }

      //custom validation check
    roleFormValidaton(): boolean {
        this.roleFormErrorObject.showRoleNameError = false;
        if (this.roleFormGroup.controls.roleName.invalid) {
            this.roleFormErrorObject.showRoleNameError = true;
            return false;
        }
        return true;
    }

    //function to format data
    onSubmit(): void {
        if (this.roleFormValidaton()) {
            let privileges = [];
            for (var i = 0; i < this.entityList.length; i++) {
                for (var j = 0; j < this.entityList[i].Operation.length; j++) {
                    if (this.entityList[i].Operation[j].IsEnabled) {
                        let obj: any = {};
                        obj.EntityName = this.entityList[i].EntityName;
                        obj.Operation = this.entityList[i].Operation[j].operation;
                        privileges.push(obj);
                    }
                }
            }
            let roleObject: any = {
                Name: this.roleFormGroup.value.roleName,
                Description: this.roleFormGroup.value.roleDescription,
                Privileges: privileges,
                Status: true,
            }
            if (this.roleEditModeOn && localStorage.getItem('roleparams')) {
                roleObject.Identifier = this.params.Routeparams.passingparams.RoleIdentifier
            }
            this.saveRecord(roleObject);
        }
    }

    //method written to save role
    async saveRecord(roleRecord) {
        let roleArray = [];
        roleArray.push(roleRecord);
        let roleService = this.injector.get(RoleService);
        let isRecordSaved = await roleService.saveRole(roleArray, this.roleEditModeOn);
        if (isRecordSaved) {
            let message = Constants.recordAddedMessage;
            if (this.roleEditModeOn) {
                message = Constants.recordUpdatedMessage;
            }
            this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
            this.navigateToListPage()
        }
    }

    //This method to fetch role privilege records
    getRoleEntityRecords() {
      let roleService = this.injector.get(RoleService);
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.EntityName;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      this.isLoaderActive = true;
      this.uiLoader.start();
      // roleService.getRoleEntities(searchParameter)
      //     .subscribe(
      //         (httpEvent: HttpEvent<any>) => {
      //             if (httpEvent.type == HttpEventType.Response) {
      //                 if (httpEvent["status"] === 200) {
      //                     this.entityList = [];
      //                     this.uiLoader.stop();
      //                     if (httpEvent['body'] != null) {
      //                         httpEvent['body'].forEach(entityObject => {
      //                             var datas: any = []
      //                             entityObject.Operations.forEach(operation => {
      //                                 let data = {
      //                                     "operation": operation,
      //                                     "IsEnabled": false
      //                                 }
      //                                 datas.push(data)
      //                             })
      //                             let entitydata = {
      //                                 "EntityName": entityObject.EntityName,
      //                                 "Operation": datas,
      //                                 "IsAllRolePrevilegeSelected": false
      //                             }
      //                             //this.entityList.push(entitydata)
      //                             this.entityList = [...this.entityList, entitydata];
      //                         });
      //                     }
      //                     if (this.roleEditModeOn == true && localStorage.getItem('roleparams')) {
      //                         this.getRoleInfoRecord();
      //                     }
      //                 }
      //                 else {
      //                     this.entityList = [];
      //                     this.uiLoader.stop();
      //                 }
      //             }
      //         },
      //         (error: HttpResponse<any>) => {
      //             this.entityList = [];
      //             this.uiLoader.stop();
      //         }
      //     );
          this.entityList = [
            {
              "EntityName" : "Dashboard",
              "Operation" : [
                {
                  "operation": "Create",
                  "IsEnabled":true
                },
                {
                  "operation": "Edit",
                  "IsEnabled":true
                },
                {
                  "operation": "Delete",
                  "IsEnabled":true
                },
                {
                  "operation": "View",
                  "IsEnabled":true
                }
              ]
            },
            {
              "EntityName" : "Widgets",
              "Operation" : [
                {
                  "operation": "Create",
                  "IsEnabled":true
                },
                {
                  "operation": "Edit",
                  "IsEnabled":true
                },
                {
                  "operation": "Delete",
                  "IsEnabled":true
                },
                {
                  "operation": "View",
                  "IsEnabled":true
                }
              ]
            },
            {
              "EntityName" : "Pages",
              "Operation" : [
                {
                  "operation": "Create",
                  "IsEnabled":true
                },
                {
                  "operation": "Edit",
                  "IsEnabled":true
                },
                {
                  "operation": "Delete",
                  "IsEnabled":true
                },
                {
                  "operation": "View",
                  "IsEnabled":true
                }
              ]
            },
            {
              "EntityName" : "Statement Definitions",
              "Operation" : [
                {
                  "operation": "Create",
                  "IsEnabled":true
                },
                {
                  "operation": "Edit",
                  "IsEnabled":true
                },
                {
                  "operation": "Delete",
                  "IsEnabled":true
                },
                {
                  "operation": "View",
                  "IsEnabled":true
                }
              ]
            }
          ];
          this.uiLoader.stop();
    }

    // Function call to patch the value at the time of edititing role--
    async getRoleInfoRecord() {
      let roleService = this.injector.get(RoleService);
      let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.GetPrivileges = true;
      if (this.RoleIdentifier != null)
          searchParameter.Identifiers = this.RoleIdentifier;
      this.rolRecord = await roleService.getRoles(searchParameter);
      this.rolRecord.forEach(roleObject => {
          this.roleFormGroup.patchValue({
              roleName: roleObject.Name,
              roleDescription: roleObject.Description,
          })
          for (let i = 0; i < this.rolRecord.length; i++) {
              this.checkedRole = this.rolRecord[i].Privileges
          }
          //console.log(this.checkedRole)
          for (let i = 0; i < this.checkedRole.length; i++) {
              for (let count = 0; count < this.entityList.length; count++) {
                  if (this.entityList[count].EntityName == this.checkedRole[i].EntityName) {
                      for (let innercount = 0; innercount < this.entityList[count].Operation.length; innercount++) {
                          if (this.entityList[count].Operation[innercount].operation == this.checkedRole[i].Operation) {
                              this.entityList[count].Operation[innercount].IsEnabled = true;
                          }
                      }
                  }
              }
          }
      });
    }

    //navigate to role list
    navigateToListPage() {
        const router = this.injector.get(Router);
        router.navigate(['roles']);
    }

    //initialization call
    ngOnInit() {
      //Validations for Role Form.
      this.roleFormGroup = this.formbuilder.group({
          roleName: [null, Validators.compose([Validators.required,
              Validators.pattern(this.onlyAlphabetsWithSpaceQuoteHyphen)])
          ],
          roleDescription: [null],
      });
      this.permissonFilterForm = this.formbuilder.group(
        { filterPermission: [null], }
      );
      this.getRoleEntityRecords();
    }

    selectAllPermissions(event) {
        this.allPermisions = event.target.checked;
        this.entityList.forEach(rolelist => {
            rolelist.IsAllRolePrevilegeSelected = event.target.checked
            rolelist.Operation.forEach(previlgeOperationList => {
                previlgeOperationList.IsEnabled = event.target.checked;
            })
        })
    }

    selectAllEntityOperations(event, entityName) {
        this.entityList.forEach(entity => {
            if (entity.EntityName == entityName) {
                entity.Operation.forEach(previlgeOperationList => {
                    previlgeOperationList.IsEnabled = event.target.checked;
                })
            }
        })
    }

    rowWiseSelection(event, operation, entityName) {
        this.entityList.forEach(entity => {
            if (entity.EntityName == entityName) {
                let operationLength = entity.Operation.length;
                let counter = 0;
                entity.Operation.forEach(previlgeOperationList => {
                    if (previlgeOperationList.IsEnabled == true) {
                        counter++;
                    }
                    // previlgeOperationList.IsEnabled = event.target.checked;
                })
                if (counter == operationLength) {
                    entity.IsAllRolePrevilegeSelected = true;
                }
                else {
                    entity.IsAllRolePrevilegeSelected = false;
                }
            }
        })
    }

}
