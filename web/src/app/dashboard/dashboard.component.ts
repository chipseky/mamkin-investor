import { Component } from '@angular/core';
import {BybitBalance, DashboardClient} from "../api-clients";
import {Observable} from "rxjs";
import {AsyncPipe} from "@angular/common";

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  imports: [
    AsyncPipe
  ],
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  balance$: Observable<BybitBalance>;

  constructor(private dashboardClient: DashboardClient) {
    this.balance$ = dashboardClient.getBalance();
  }
}
