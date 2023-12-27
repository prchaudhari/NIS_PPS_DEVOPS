import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/modules/shared.module';
import { DashboardDesignerRoutingModule } from './dashboard-designer-routing.module';
import { AddDashboardDesignerComponent } from './add-dashboard-designer/add-dashboard-designer.component';
import { ViewDashboardDesignerComponent } from './view-dashboard-designer/view-dashboard-designer.component';
import { GridsterModule } from 'angular-gridster2';
import { LayoutModule } from '../layout/layout.module';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { DynamicModule } from 'ng-dynamic-component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  CustomerInformationComponent, FSPDetailsComponent, PaymentSummaryComponent, PPSHeadingComponent, PPSDetailsComponent,
  AccountInformationComponent, ImageComponent, VideoComponent, SummaryAtGlanceComponent,
  TransactionDetailsComponent, SavingAvailableBalanceComponent, CurrentAvailableBalanceComponent, SavingTransactionDetailsComponent, SpendindTrendsComponent,
  TopIncomeSourcesComponent, SavingTrendsComponent, AnalyticsWidgetComponent, ReminderAndRecommComponent, SavingTrendsPreviewComponent, AnalyticsWidgetPreviewComponent, 
  SpendindTrendsPreviewComponent, DynamicLineChartWidgetComponent, DynamicBarChartWidgetComponent, DynamicPieChartWidgetComponent, DynamicHhtmlComponent, StaticHtmlComponent, PageBreakComponent, SegmentBasedContentComponent,
  DynamicBarChartWidgetPreviewComponent, DynamicLineChartWidgetPreviewComponent, DynamicPieChartWidgetPreviewComponent,
  SpecialMessageComponent, PersonalLoanInsuranceMessageComponent,
   CSAgentLogoComponent} from './widgetComponent/widgetComponent';
import { PageDesignPreviewComponent } from './page-design-preview/page-design-preview.component';
import { RichTextEditorAllModule } from '@syncfusion/ej2-angular-richtexteditor';
import { SafeHtmlPip } from '../shared/pagepreview/pagepreview.component';
import { MatIconModule } from '@angular/material/icon';
@NgModule({
  declarations: [AddDashboardDesignerComponent, PaymentSummaryComponent, PPSHeadingComponent, ViewDashboardDesignerComponent, CustomerInformationComponent, FSPDetailsComponent, PPSDetailsComponent,
    AccountInformationComponent, ImageComponent, VideoComponent, SummaryAtGlanceComponent, TransactionDetailsComponent,
    SavingAvailableBalanceComponent, CurrentAvailableBalanceComponent, SavingTransactionDetailsComponent,
    SpendindTrendsComponent, TopIncomeSourcesComponent, SavingTrendsComponent, AnalyticsWidgetComponent, ReminderAndRecommComponent, SavingTrendsPreviewComponent, AnalyticsWidgetPreviewComponent, 
    SpendindTrendsPreviewComponent, DynamicBarChartWidgetComponent, DynamicLineChartWidgetComponent, DynamicPieChartWidgetComponent, DynamicHhtmlComponent, StaticHtmlComponent, PageBreakComponent, SegmentBasedContentComponent,
    DynamicBarChartWidgetPreviewComponent, DynamicLineChartWidgetPreviewComponent, DynamicPieChartWidgetPreviewComponent,
    SpecialMessageComponent, PersonalLoanInsuranceMessageComponent, 
    PageDesignPreviewComponent, CSAgentLogoComponent, ],
  imports: [
    CommonModule, DashboardDesignerRoutingModule, RouterModule, SharedModule, FormsModule, ReactiveFormsModule,
    GridsterModule, LayoutModule, MatSortModule, MatTableModule, MatPaginatorModule, DynamicModule, RichTextEditorAllModule, MatIconModule
  ],
  entryComponents: [
    CustomerInformationComponent, PPSDetailsComponent, FSPDetailsComponent, PaymentSummaryComponent, PPSHeadingComponent, AccountInformationComponent, ImageComponent, VideoComponent, SummaryAtGlanceComponent, TransactionDetailsComponent, SavingAvailableBalanceComponent,
    CurrentAvailableBalanceComponent, SavingTransactionDetailsComponent, SpendindTrendsComponent, TopIncomeSourcesComponent, SavingTrendsComponent, AnalyticsWidgetComponent,
    ReminderAndRecommComponent, SavingTrendsPreviewComponent, AnalyticsWidgetPreviewComponent, SpendindTrendsPreviewComponent,
    DynamicLineChartWidgetComponent, DynamicBarChartWidgetComponent, DynamicPieChartWidgetComponent, DynamicHhtmlComponent, StaticHtmlComponent, PageBreakComponent, SegmentBasedContentComponent,
    DynamicBarChartWidgetPreviewComponent, DynamicLineChartWidgetPreviewComponent, DynamicPieChartWidgetPreviewComponent,
    SpecialMessageComponent, PersonalLoanInsuranceMessageComponent, 
    PageDesignPreviewComponent, CSAgentLogoComponent, ],
})
export class DashboardDesignerModule { }
