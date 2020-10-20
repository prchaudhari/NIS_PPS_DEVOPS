import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Constants } from '../../../shared/constants/constants';
import { ErrorMessageConstants } from '../../../shared/constants/constants';
import { MessageDialogService } from '../../../shared/services/mesage-dialog.service';
import { DynamicWidgetService } from '../dynamicWidget.service';
import { DynamicWidget } from '../dynamicwidget';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { ToolbarService, LinkService, ImageService, HtmlEditorService, RichTextEditor } from '@syncfusion/ej2-angular-richtexteditor';
import { TemplateService } from '../../template/template.service';

export interface ListElement {
  series: string;
  displayName: string;
}

const List_Data: ListElement[] = [
  //{ series: 'Field Name 01', displayName: 'FN 01' },
  //{ series: 'Field Name 02', displayName: 'FN 02' },
  //{ series: 'Field Name 03', displayName: 'FN 03' },
  //{ series: 'Field Name 04', displayName: 'FN 04' },
  //{ series: 'Field Name 05', displayName: 'FN 05' },
];
@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss'],
  providers: [ToolbarService, LinkService, ImageService, HtmlEditorService]
})
export class AddComponent implements OnInit {
  //html editor code
  htmlContent = '';
  config: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: '15rem',
    minHeight: '5rem',
    placeholder: 'Enter text here...',
    translate: 'no',
    defaultParagraphSeparator: 'p',
    defaultFontName: 'Arial',
    toolbarHiddenButtons: [
      //['bold']
    ],
    customClasses: [
      {
        name: "quote",
        class: "quote",
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: "titleText",
        class: "titleText",
        tag: "h1",
      },
    ]
  };
  public isDefault: boolean = true;
  public isCustome: boolean = false;
  public dynamicWidgetDetails: DynamicWidget;
  public params;
  public userClaimsRolePrivilegeOperations: any[] = [];
  isDefaultClicked() {
    this.isDefault = true;
    this.isCustome = false;
  }
  isCustomeClicked() {
    this.isDefault = false;
    this.isCustome = true;
  }
  public filterConditions: any[] = [];
  public
  public isTheme1Active: boolean = false;
  public isTheme2Active: boolean = false;
  public isTheme3Active: boolean = false;
  public isTheme4Active: boolean = false;
  public isTheme5Active: boolean = false;
  public isTheme0Active: boolean = true;
  //Functions call to click the theme of the page--
  theme1() {
    this.isTheme1Active = true;
    this.isTheme2Active = false;
    this.isTheme3Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme2() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = true;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme3() {
    this.isTheme1Active = false;
    this.isTheme3Active = true;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;

  }
  theme4() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = true;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme5() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = true;
    this.isTheme0Active = false;
  }
  theme0() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = true;
  }
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;
  public DynamicWidgetForm: FormGroup;
  public pageTypeList: any[] = [{ "PageTypeName": "Select Page Type", "Identifier": 0 }];
  public entityList: any[] = [{ "Name": "Select Entity", "Identifier": 0 }];
  public entityFieldList: any[] = [{ "Name": "Select", "Identifier": 0 }];
  displayedColumns: string[] = ['series', 'displayName', 'actions'];
  public lineBarGraphList: any[] = [];
  public widgetFilterlist: any[] = [];
  public displayWidgetFilterlist: any[] = [];


  public conditionList: any[] = [
    { "Name": "Select", "Identifier": "0" },
    { "Name": "Equals To", "Identifier": "EqualsTo" },
    { "Name": "Not Equals To", "Identifier": "NotEqualsTo" },
    { "Name": "Less Than", "Identifier": "LessThan" },
    { "Name": "Greater Than", "Identifier": "GreaterThan" },
    { "Name": "Contains", "Identifier": "Contains" },
    { "Name": "Not Contains", "Identifier": "NotContains" }
  ];
  public updateOperationMode: boolean;
  dataSource = new MatTableDataSource<any>(this.lineBarGraphList);

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
  }

  tableHeader = [
    //{ fn: 'Field Name 01', name: 'Date', sorting: 'Yes' },
    //{ fn: 'Field Name 02', name: 'Amout', sorting: 'No' },
    //{ fn: 'Field Name 03', name: 'Narration', sorting: 'Yes' },
    //{ fn: 'Field Name 04', name: 'Balance', sorting: 'No' },
    //{ fn: 'Field Name 05', name: 'Product', sorting: 'Yes' },
    //{ fn: 'Field Name 06', name: 'User', sorting: 'Yes' },
    //{ fn: 'Field Name 07', name: 'Role', sorting: 'Yes' },
  ];

  formList = [
    //{ fn: 'Field Name 01', dn: 'Customer Name' },
    //{ fn: 'Field Name 02', dn: 'Customer Id' },
    //{ fn: 'Field Name 03', dn: 'Balance' },
    //{ fn: 'Field Name 01', dn: 'Account Id' },
  ];

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.tableHeader, event.previousIndex, event.currentIndex);
    moveItemInArray(this.formList, event.previousIndex, event.currentIndex);
  }

  //select widget type radio

  selectedLink: string = "Form";

  setWidgetType(e: string): void {
    //  this.selectRowsOption = '';
    this.selectedLink = e;
  }

  isSelected(name: string): boolean {
    if (!this.selectedLink) { // if no radio button is selected, always return false so every nothing is shown  
      return false;
    }
    return (this.selectedLink === name); // if current radio button is selected, return true, else return false 
  }

  navigateToListPage() {
    this._router.navigate(['dynamicwidget']);
  }

  constructor(
    private _router: Router,
    private fb: FormBuilder,
    private injector: Injector,
    private _messageDialogService: MessageDialogService,
    private dynamicWidgetService: DynamicWidgetService
  ) {
    this.dynamicWidgetDetails = new DynamicWidget;
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/dynamicwidget/Add')) {
          localStorage.removeItem("dynamicWidgetEditRouteparams");
        }
      }
    });

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/dynamicwidget')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('dynamicWidgetEditRouteparams'));
          if (localStorage.getItem('dynamicWidgetEditRouteparams')) {
            this.dynamicWidgetDetails.Identifier = this.params.Routeparams.passingparams.DynamicWidgetIdentifier;
            //this.getWidgetDetails();
          }
        } else {
          localStorage.removeItem("dynamicWidgetEditRouteparams");
        }
      }
    });
  }

  ngOnInit() {
    this.DynamicWidgetForm = this.fb.group({
      WidgetName: [null],
      PageType: [0],
      Entity: [0],
      WidgetTitle: [null],
      FormEntityField: [0],
      FormFieldDisplayName: [null],
      TableHeaderName: [null],
      TableEntityField: [0],
      TableIsSorting: [false],
      LineBarEntityField: [0],
      LineBarFieldDisplayName: [null],
      PieSeries: [0],
      PieValue: [0],
      FilterConditionOperator: ["0"],
      FilterField: [0],
      FilterOperator: [0],
      FilterValue: [null],
      FontColor: [null],
      FontSize: [null],
      FontWeight: [0],
      FontType: [0],
      HeaderColor: [null],
      HeaderSize: [null],
      HeaderWeight: [0],
      HeaderType: [0],
      DataColor: [null],
      DataSize: [null],
      DataWeight: [0],
      DataType: [0],
      HTMLEntityField: [0]
    });
    if (localStorage.getItem('dynamicWidgetEditRouteparams')) {
      this.updateOperationMode = true;
      this.dynamicWidgetDetails.Identifier = this.params.Routeparams.passingparams.DynamicWidgetIdentifier;
      this.getWidgetDetails();
    }
    else {
      this.updateOperationMode = false;

    }
    this.getPageTypes();
    this.getEntities();
  }

  async getWidgetDetails() {
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = "Id";
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.Identifier = this.dynamicWidgetDetails.Identifier;
    searchParameter.IsStatementPagesRequired = true;
    var response = await this.dynamicWidgetService.getDynamicWidgets(searchParameter);
    this.dynamicWidgetDetails = response.List[0];
    this.setWidgetType(this.dynamicWidgetDetails.WidgetType);
    var themeCSS: any = {};
    this.getEntityField(this.dynamicWidgetDetails.EntityId);
    if (this.dynamicWidgetDetails.ThemeCSS != null && this.dynamicWidgetDetails.ThemeCSS != '') {
      themeCSS = JSON.parse(this.dynamicWidgetDetails.ThemeCSS);
      this.DynamicWidgetForm.patchValue({
        WidgetName: this.dynamicWidgetDetails.WidgetName,
        PageType: this.dynamicWidgetDetails.PageTypeId,
        Entity: this.dynamicWidgetDetails.EntityId,
        WidgetTitle: this.dynamicWidgetDetails.Title,
        FontColor: themeCSS.FontColor,
        FontSize: themeCSS.FontSize,
        FontWeight: themeCSS.FontWeight,
        FontType: themeCSS.FontType,
        HeaderColor: themeCSS.HeaderColor,
        HeaderSize: themeCSS.HeaderSize,
        HeaderWeight: themeCSS.HeaderWeight,
        HeaderType: themeCSS.HeaderType,
        DataColor: themeCSS.DataColor,
        DataSize: themeCSS.DataSize,
        DataWeight: themeCSS.DataWeight,
        DataType: themeCSS.DataType
      });
    }
    this.isDefault = this.dynamicWidgetDetails.ThemeType == "Default" ? true : false;

    this.isCustome = this.dynamicWidgetDetails.ThemeType == "Default" ? false : true;

    this.DynamicWidgetForm.patchValue({
      WidgetName: this.dynamicWidgetDetails.WidgetName,
      PageType: this.dynamicWidgetDetails.PageTypeId,
      Entity: this.dynamicWidgetDetails.EntityId,
      WidgetTitle: this.dynamicWidgetDetails.Title,
    });

  }

  async getPageTypes() {
    let dynamicWidgetService = this.injector.get(TemplateService);
    var data = await dynamicWidgetService.getPageTypes();
    data.forEach(item => {
      this.pageTypeList.push(item);
    })
    if (this.pageTypeList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
        if (data == true) {
          this.getPageTypes();
        }
      });
    }
  }

  async getEntities() {

    var data = await this.dynamicWidgetService.getEntities();
    data.forEach(item => {
      this.entityList.push(item);
    })
    if (this.entityList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
      });
    }

  }

  public onPageTypeSelected(event) {
    const value = event.target.value;
    if (value == "0") {
      //this.filterPageTypeId = 0;
    }
    else {
      // this.filterPageTypeId = Number(value);
    }
  }

  public onEntitySelected(event) {
    const value = event.target.value;
    if (value == "0") {
      this.entityFieldList = [];
    }
    else {
      //this.getEntityField(value);
    }
  }

  async getEntityField(value) {
    var data = await this.dynamicWidgetService.getEntityFields(value);
    this.entityFieldList = [{ "Name": "Select", "Identifier": 0 }];
    data.forEach(item => {
      this.entityFieldList.push(item);
    })
    if (this.entityFieldList.length == 0) {
      let message = ErrorMessageConstants.getNoRecordFoundMessage;
      this._messageDialogService.openDialogBox('Error', message, Constants.msgBoxError).subscribe(data => {
      });
    }

  }

  public onEntityFieldSelected(event, widgetType) {

    const value = event.target.value;
    if (value == "0") {
      //this.filterPageTypeId = 0;
    }
    else {
      // this.filterPageTypeId = Number(value);
    }
  }

  public addFieldDetails(widgetType) {
    var object: any;
    if (widgetType == 'Form') {

      var index = this.formList.findIndex(i => { return i.FieldId == this.DynamicWidgetForm.value.FormEntityField });
      if (index >= 0) {
        this._messageDialogService.openDialogBox('Error', "Field already added", Constants.msgBoxError);
      }
      else {
        var field = this.entityFieldList.filter(i => { return i.Identifier == this.DynamicWidgetForm.value.FormEntityField });
        object = {
          "DisplayName": this.DynamicWidgetForm.value.FormFieldDisplayName,
          "FieldId": this.DynamicWidgetForm.value.FormEntityField,
          "FieldName": field[0].Name
        }
        this.formList.push(object);
        this.DynamicWidgetForm.patchValue({
          FormFieldDisplayName: null,
          FormEntityField: 0,
        })
      }
    }
    else if (widgetType == 'Table') {
      var index = this.tableHeader.findIndex(i => { return i.FieldId == this.DynamicWidgetForm.value.TableEntityField });
      if (index >= 0) {
        this._messageDialogService.openDialogBox('Error', "Field already added", Constants.msgBoxError);
      }
      else {
        var field = this.entityFieldList.filter(i => { return i.Identifier == this.DynamicWidgetForm.value.TableEntityField });
        object = {
          "HeaderName": this.DynamicWidgetForm.value.TableHeaderName,
          "FieldId": this.DynamicWidgetForm.value.TableEntityField,
          "FieldName": field[0].Name,
          "IsSorting": this.DynamicWidgetForm.value.TableIsSorting
        }
        this.tableHeader.push(object);
        this.DynamicWidgetForm.patchValue({
          TableHeaderName: null,
          TableEntityField: 0,
          TableIsSorting: false
        })
      }
    }
    else if (widgetType = 'LineBraGraph') {
      var index = this.lineBarGraphList.findIndex(i => { return i.FieldId == this.DynamicWidgetForm.value.LineBarEntityField });
      if (index >= 0) {
        this._messageDialogService.openDialogBox('Error', "Field already added", Constants.msgBoxError);
      }
      else {
        var field = this.entityFieldList.filter(i => { return i.Identifier == this.DynamicWidgetForm.value.LineBarEntityField });
        object = {
          "DisplayName": this.DynamicWidgetForm.value.LineBarFieldDisplayName,
          "FieldId": this.DynamicWidgetForm.value.LineBarEntityField,
          "FieldName": field[0].Name
        }
        this.lineBarGraphList.push(object);
        this.dataSource = new MatTableDataSource<any>(this.lineBarGraphList);
        this.DynamicWidgetForm.patchValue({
          LineBarFieldDisplayName: null,
          LineBarEntityField: 0,
        })
      }
    }

  }

  public AddFilterCondition() {
    var object: any = {};
    object.FieldId = this.DynamicWidgetForm.value.FilterField
    object.Operator = this.DynamicWidgetForm.value.FilterConditionOperator
    object.ConditionalOperator = this.DynamicWidgetForm.value.FilterConditionOperator
    object.Sequence = this.displayWidgetFilterlist.length + 1;
    object.Value = this.DynamicWidgetForm.value.FilterValue;
    this.widgetFilterlist.push(object);
    object.OperatorName = this.conditionList.filter(item => item.Operator == object.Operator)[0];
    object.FieldName = this.entityFieldList.filter(item => item.Identifier == object.FieldId)[0];
    this.displayWidgetFilterlist.push(object);
  }

  async saveWidgetDetails() {
    var widget: DynamicWidget;
    widget = new DynamicWidget();
    widget.WidgetName = this.DynamicWidgetForm.value.WidgetName;
    widget.WidgetType = this.selectedLink;
    widget.PageTypeId = this.DynamicWidgetForm.value.PageType;
    widget.EntityId = this.DynamicWidgetForm.value.Entity;
    widget.ThemeType = this.isDefault == true ? "Default" : "Custome";
    widget.Title = this.DynamicWidgetForm.value.WidgetTitle;
    //if (this.selectedLink == 'Form') {
    //  widget.WidgetSettings = JSON.stringify(this.formList);
    //}
    //else if (this.selectedLink == 'Table') {
    //  widget.WidgetSettings = JSON.stringify(this.tableHeader);
    //}
    //else if (this.selectedLink == 'LineBarGraph') {
    //  widget.WidgetSettings = JSON.stringify(this.lineBarGraphList);
    //}
    //else if (this.selectedLink == 'Pie') {

    //}
    //else if (this.selectedLink == 'Html') {

    //}

    //widget.ThemeCSS = '';
    //var themeObject = {
    //  "FontColor": this.DynamicWidgetForm.value.FontColor,
    //  "FontSize": this.DynamicWidgetForm.value.FontSize,
    //  "FontWeight": this.DynamicWidgetForm.value.FontWeight,
    //  "FontType": this.DynamicWidgetForm.value.FontType,
    //  "HeaderColor": this.DynamicWidgetForm.value.HeaderColor,
    //  "HeaderSize": this.DynamicWidgetForm.value.HeaderSize,
    //  "HeaderWeight": this.DynamicWidgetForm.value.HeaderWeight,
    //  "HeaderType": this.DynamicWidgetForm.value.HeaderType,
    //  "DataColor": this.DynamicWidgetForm.value.DataColor,
    //  "DataSize": this.DynamicWidgetForm.value.DataSize,
    //  "DataWeight": this.DynamicWidgetForm.value.DataWeight,
    //  "DataType": this.DynamicWidgetForm.value.DataType
    //}
    //widget.ThemeCSS = JSON.stringify(themeObject);
    //widget.WidgetFilterSettings = '';

    var userid = localStorage.getItem('UserId');
    widget.CreatedBy = Number(userid);
    var widgetList = [];
    widgetList.push(widget);
    let isRecordSaved = await this.dynamicWidgetService.saveDynamicWidget(widgetList, this.updateOperationMode);
    if (isRecordSaved) {
      let message = Constants.recordAddedMessage;
      if (this.updateOperationMode) {
        message = Constants.recordUpdatedMessage;
      }
      this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
      this.navigateToListPage()
    }
  }

  public disableSave() {

    if (this.DynamicWidgetForm.value.WidgetName == null || this.DynamicWidgetForm.value.WidgetName == '') {
      return true;
    }
    if (this.DynamicWidgetForm.value.WidgetTitle == null || this.DynamicWidgetForm.value.WidgetTitle == '') {
      return true;
    }
    if (this.DynamicWidgetForm.value.PageType == null || this.DynamicWidgetForm.value.PageType == 0) {
      return true;
    }
    if (this.DynamicWidgetForm.value.Entity == null || this.DynamicWidgetForm.value.Entity == 0) {
      return true;
    }
    return false;
  }

  navigationToDesigner() {
    var pagetype: any = {}
    pagetype = this.pageTypeList.filter(item => { return item.Identifier == this.DynamicWidgetForm.value.PageType });
    var entity: any = {}
    entity = this.entityList.filter(item => { return item.Identifier == this.DynamicWidgetForm.value.Entity });
    let queryParams = {
      Routeparams: {
        passingparams: {
          "WidgetName": this.DynamicWidgetForm.value.WidgetName,
          "WidgetType": this.selectedLink,
          "PageTypeId": this.DynamicWidgetForm.value.PageType,
          "PageTypeName": pagetype.PageTypeName,
          "EntityId": this.DynamicWidgetForm.value.Entity,
          "EntityName": entity.Name,
          "Title": this.DynamicWidgetForm.value.WidgetTitle,
          "updateOperationMode": this.updateOperationMode,
          "DynamicWidgetIdentifier": this.updateOperationMode ? this.dynamicWidgetDetails.Identifier : 0,
        }
      }
    }
    localStorage.setItem("widgetDesignerParams", JSON.stringify(queryParams));
    this._router.navigate(['dynamicwidget', 'AddDesinger']);
  }
}
