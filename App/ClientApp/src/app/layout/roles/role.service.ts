import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { URLConfiguration } from 'src/app/shared/urlConfiguration/urlconfiguration';
import { HttpClientService } from 'src/app/core/services/httpClient.service';
import { Constants } from 'src/app/shared/constants/constants';
import { Role } from 'src/app/layout/roles/role';

@Injectable({
  providedIn: 'root'
})
export class RoleService {

  public accessToken;
  public roleList: Role[] = [];
  public isRecordFound: boolean = false;
  public isRecordSaved: boolean = false;
  public isDependencyPresent: boolean = false;
  public isRecordDeleted = {};

  constructor(private http: HttpClient,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService) { }

  //method to call api of get role.
  async getRoles(searchParameter): Promise<Role[]> {
      let httpClientService = this.injector.get(HttpClientService);
      let requestUrl = URLConfiguration.roleGetUrl;
      this.uiLoader.start();
      await httpClientService.CallHttp("POST", requestUrl, searchParameter).toPromise()
          .then((httpEvent: HttpEvent<any>) => {
              if (httpEvent.type == HttpEventType.Response) {
                  if (httpEvent["status"] === 200) {
                      this.roleList = [];
                      this.uiLoader.stop();
                      httpEvent['body'].forEach(roleObject => {
                          this.roleList = [...this.roleList, roleObject];
                      });
                  }
                  else {
                      this.roleList = [];
                      this.uiLoader.stop();
                  }
              }
          }, (error: HttpResponse<any>) => {
              this.roleList = [];
              this.uiLoader.stop();
              if (error["error"] != null) {
                  let errorMessage = error["error"].Error["Message"];
                  this._messageDialogService.openDialogBox('Error', errorMessage, Constants.msgBoxError);
              }
          });
      return <Role[]>this.roleList
  }

  //service method to fetch roleprivilege records
  public getRolePrivileges(searchParameter): Observable<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.roleGetRolePrivileges;
    return httpClientService.CallHttp("POST", requestUrl, searchParameter);
}

//service method to fetch entities records
public getRoleEntities(searchParameter): Observable<any> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.roleGetEntities;
    return httpClientService.CallHttp("POST", requestUrl, searchParameter);
}

//service method to save or update roleprivilege records
public async saveRole(postData, roleEditModeOn): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let requestUrl = URLConfiguration.roleAddUrl;
    if (roleEditModeOn) {
        requestUrl = URLConfiguration.roleUpdateUrl;
    }
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl, postData).toPromise()
        .then((httpEvent: HttpEvent<any>) => {
            if (httpEvent.type == HttpEventType.Response) {
                this.uiLoader.stop();
                if (httpEvent["status"] === 200) {
                    this.isRecordSaved = true;
                }
                else {
                    this.isRecordSaved = false;
                }
            }
        }, (error: HttpResponse<any>) => {
            this.isRecordSaved = false;
            this.uiLoader.stop();
        });
    return <boolean>this.isRecordSaved;
}

//method to call api of dependency check allowance.
public async checkDependency(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
        identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.roleCheckDeleteDependencyUrl + '?Identifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
        .then((httpEvent: HttpEvent<any>) => {
            if (httpEvent.type == HttpEventType.Response) {
                this.uiLoader.stop();
                if (httpEvent['body'] == true) {
                    this.isDependencyPresent = true;
                }
                else {
                    this.isDependencyPresent = false;
                }
            }
        }, (error: HttpResponse<any>) => {
            this.uiLoader.stop();
            this.isDependencyPresent = false;
        });
    return <boolean>this.isDependencyPresent;
}

//method to call api of delete role.
public async deleteRole(postData): Promise<boolean> {
    let httpClientService = this.injector.get(HttpClientService);
    let identifier = null;
    if (postData.length > 0) {
        identifier = postData[0].Identifier;
    }
    let requestUrl = URLConfiguration.roleDeleteUrl + '?Identifier=' + identifier;
    this.uiLoader.start();
    await httpClientService.CallHttp("POST", requestUrl).toPromise()
        .then((httpEvent: HttpEvent<any>) => {
            if (httpEvent.type == HttpEventType.Response) {
                this.uiLoader.stop();
                if (httpEvent["status"] === 200) {
                    this.isRecordDeleted = true
                }
                else {
                    this.isRecordDeleted = false
                }
            }
        }, (error: HttpResponse<any>) => {
            this.uiLoader.stop();
            this.isRecordDeleted = false
        });
    return <boolean>this.isRecordDeleted;
}

}
