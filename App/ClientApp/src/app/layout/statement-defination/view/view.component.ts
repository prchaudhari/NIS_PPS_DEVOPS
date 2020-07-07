import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Constants } from 'src/app/shared/constants/constants';
import { ErrorMessageConstants } from 'src/app/shared/constants/constants';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { StatementService } from '../statement.service';
import { Statement } from '../statement';
@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {

  public isCollapsedDetails: boolean = false;
  public isCollapsedPermissions: boolean = true;
  public statement: any = {};
  public params;


  constructor(
    private _router: Router,
    private injector: Injector,
    private _messageDialogService: MessageDialogService,
    private statementService: StatementService,
  ) {
    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/statementdefination/Add')) {
          localStorage.removeItem("statementparams");
        }
      }

    });

    _router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/statementdefination')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('statementparams'));
          if (localStorage.getItem('statementparams')) {
            this.statement.Identifier = this.params.Routeparams.passingparams.StatementIdentifier;
            this.getStatementRecords();

          }
        } else {
          localStorage.removeItem("statementparams");
        }
      }

    });

  }
  navigateToListPage() {
    this._router.navigate(['/statementdefination']);
  }
  async getStatementRecords() {
    let statementService = this.injector.get(StatementService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.Identifier = this.statement.Identifier;
    searchParameter.IsStatementPagesRequired = true;
    var statements = await statementService.getStatements(searchParameter);
    this.statement = statements[0];
  }
  ngOnInit() {
  }
  navigateToStatementEdit() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "StatementIdentifier": this.statement.Identifier,
        },
        filteredparams: {
          //passing data using json stringify.
          "StatementName": this.statement.Name != null ? this.statement.Name : ""
        }
      }
    }
    localStorage.setItem("statementparams", JSON.stringify(queryParams))
    const router = this.injector.get(Router);
    router.navigate(['statementdefination', 'Edit']);
  }

  deleteStatement() {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let roleData = [{
          "Identifier": this.statement.Identifier,
        }];

        let isDeleted = await this.statementService.deleteStatement(roleData);
        if (isDeleted) {
          let messageString = Constants.recordDeletedMessage;
          this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
          this._router.navigate(['statementdefination']);
        }
      }
    });
  }
}

