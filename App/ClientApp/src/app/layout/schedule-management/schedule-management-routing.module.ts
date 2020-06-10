import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { ViewComponent } from './view/view.component';
import { AddComponent } from './add/add.component';
import { HistoryComponent } from './history/history.component';
const routes: Routes = [
    {
        path: '',
        children: [
            { path: '', component: ListComponent },
            { path: 'Add', component: AddComponent },
            { path: 'View', component: ViewComponent },
            { path: 'List', component: ListComponent },
            { path: 'History', component: HistoryComponent },
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ScheduleManagementRoutingModule { }
