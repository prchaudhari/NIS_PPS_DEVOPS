import { Component, OnInit, ViewChild, Injector } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Constants } from 'src/app/shared/constants/constants';
import { TenantConfigurationService } from './tenantConfiguration.service';
import { HttpClient,HttpResponse, HttpEvent, HttpEventType } from '@angular/common/http';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { MsgBoxComponent } from 'src/app/shared/modules/message/messagebox.component';
import { Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { TenantConfiguration } from './tenatconfiguration';
import * as $ from 'jquery';
@Component({
  selector: 'app-tenant-configuration',
  templateUrl: './tenant-configuration.component.html',
  styleUrls: ['./tenant-configuration.component.scss']
})
export class TenantConfigurationComponent implements OnInit {

  tenantConfigurationForm: FormGroup;
  public validUrlRegexPattern = '^(http(s?):\/\/)?([0-9]{2,3}).([0-9]{2,3}).([0-9]{2,3}).([0-9]{1,3})([:0-9]{3,6})?$';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public tenantConfigurationEditModeOn: boolean = false;
  public PriorityLevel: number = 0;
  public ConcurrencyCount: number = 0;
  public params;
  public TenantConfigurationIdentifier: number = 0;
  public baseURL: string = ConfigConstants.BaseURL;

  // Object created to initlialize the error boolean value.
  public tenantConfigurationFormErrorObject: any = {
    showPriorityLevelError: false,
    showConcurrencyCountError: false,
  };
  public setting: TenantConfiguration;
  //getters of render engine Form group
  get TenantConfigurationName() {
    return this.tenantConfigurationForm.get('TenantConfigurationName');
  }
  get TenantConfigurationDescription() {
    return this.tenantConfigurationForm.get('TenantConfigurationDescription');
  }
  get TenantConfigurationOutputPDFPath() {
    return this.tenantConfigurationForm.get('TenantConfigurationOutputPDFPath');
  }
  get TenantConfigurationOutputHTMLPath() {
    return this.tenantConfigurationForm.get('TenantConfigurationOutputHTMLPath');
  }
  get TenantConfigurationInputDataSourcePath() {
    return this.tenantConfigurationForm.get('TenantConfigurationInputDataSourcePath');
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
    private formBuilder: FormBuilder,
    private http: HttpClient,
    private tenantConfigurationService: TenantConfigurationService,
    private _dialogService: DialogService,
    private spinner: NgxUiLoaderService,
    private router: Router,
    private _messageDialogService: MessageDialogService,
    private localstorageservice: LocalStorageService,
    private _http: HttpClient,
    private injector: Injector) {
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/renderengines')) {
          //set passing parameters to localstorage.
          if (localStorage.getItem('tenantConfigurationEditRouteparams')) {
            this.params = JSON.parse(localStorage.getItem('tenantConfigurationEditRouteparams'));
            this.TenantConfigurationIdentifier = this.params.Routeparams.passingparams.TenantConfigurationIdentifier;
            this.tenantConfigurationEditModeOn = true;
          } else {
            this.tenantConfigurationEditModeOn = false;
          }
        } else {
          localStorage.removeItem("tenantConfigurationEditRouteparams");
        }
      }
    });
  }

  ngOnInit() {
    // Render engine form validations.
    this.tenantConfigurationForm = this.formBuilder.group({
      TenantConfigurationName: [null, Validators.compose([Validators.required])],
      TenantConfigurationDescription: [null, Validators.compose([Validators.required])],
      TenantConfigurationOutputPDFPath: [null, Validators.compose([Validators.required])],
      TenantConfigurationOutputHTMLPath: [null, Validators.compose([Validators.required])],
      TenantConfigurationInputDataSourcePath: [null, Validators.compose([Validators.required])],
    });
      this.getTenantConfigurationDetails();
  }

  async getTenantConfigurationDetails() {
    this.spinner.start();
    var AssetSearchParameter;
    this._http.post(this.baseURL + 'AssetSetting/list', AssetSearchParameter).subscribe(
      data => {
        this.setting= <TenantConfiguration>data[0];
        this.spinner.stop();
      
          this.tenantConfigurationForm.patchValue({
            TenantConfigurationDescription: this.setting.Description,
            TenantConfigurationOutputPDFPath: this.setting.OutputPDFPath,
            TenantConfigurationOutputHTMLPath: this.setting.OutputHTMLPath,
            TenantConfigurationInputDataSourcePath: this.setting.InputDataSourcePath
          });
      

      },
      error => {
        $('.overlay').show();
        this._messageDialogService.openDialogBox('Error', error.error.Message, Constants.msgBoxError);
        this.spinner.stop();
      });
   
  };

  saveButtonValidation(): boolean {
    if (this.tenantConfigurationForm.controls.TenantConfigurationDescription.invalid) {
      return true;
    }
    if (this.tenantConfigurationForm.controls.TenantConfigurationOutputPDFPath.invalid) {
      return true;
    }
    if (this.tenantConfigurationForm.controls.TenantConfigurationOutputHTMLPath.invalid) {
      return true;
    }
    if (this.tenantConfigurationForm.controls.TenantConfigurationInputDataSourcePath.invalid) {
      return true;
    }
    return false;
  }

  onConcurrencyCountSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.tenantConfigurationFormErrorObject.showConcurrencyCountError = true;
      this.ConcurrencyCount = 0;
    }
    else {
      this.tenantConfigurationFormErrorObject.showConcurrencyCountError = false;
      this.ConcurrencyCount = Number(value);
    }
  }

  onPriorityLevelSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.tenantConfigurationFormErrorObject.showPriorityLevelError = true;
      this.PriorityLevel = 0;
    }
    else {
      this.tenantConfigurationFormErrorObject.showPriorityLevelError = false;
      this.PriorityLevel = Number(value);
    }
  }

  navigateToTenantConfigurationList() {
    this.router.navigate(['renderengines']);
  }

  onSubmit() {
    let tenantConfigurationObj: any = {
      "TenantConfigurationDescription": this.tenantConfigurationForm.value.TenantConfigurationDescription.trim(),
      "URL": this.tenantConfigurationForm.value.TenantConfigurationOutputPDFPath.trim(),
      "PriorityLevel": this.PriorityLevel,
      "NumberOfThread": this.ConcurrencyCount
    };
    if (this.tenantConfigurationEditModeOn) {
      tenantConfigurationObj.Identifier = this.TenantConfigurationIdentifier;
    }
    this.saveTenantConfigurationRecord(tenantConfigurationObj);
  }

  //Api called here to save render engine record
  async saveTenantConfigurationRecord(tenantConfigurationObj) {
    var TenantConfigurationArr = [];
    TenantConfigurationArr.push(tenantConfigurationObj);
    let isRecordSaved = await this.tenantConfigurationService.saveTenantConfiguration(TenantConfigurationArr, this.tenantConfigurationEditModeOn);
    if (isRecordSaved) {
      let message = Constants.recordAddedMessage;
      if (this.tenantConfigurationEditModeOn) {
        message = Constants.recordUpdatedMessage;
      }
      this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
      this.navigateToTenantConfigurationList();
      localStorage.removeItem("tenantConfigurationEditRouteparams");
    }
  }

}
