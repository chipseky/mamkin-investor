import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TradesComponent } from './trades/trades.component';
import {TradeEventsComponent} from "./trade-events/trade-events.component";

export const routes: Routes = [
    {
        path: '',
        component: DashboardComponent
    },
    {
        path: 'trades',
        component: TradesComponent
    },
    {
      path: 'trade-events',
      component: TradeEventsComponent
    }
];
