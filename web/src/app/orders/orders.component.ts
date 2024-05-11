import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {MatTableModule} from "@angular/material/table";
import {OrdersClient, OrdersType} from "../api-clients";
import {MatPaginator, MatPaginatorModule} from "@angular/material/paginator";
import {MatFormField, MatLabel} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {OrdersDataSource} from "../datasources/orders-datasource";
import {debounceTime, distinctUntilChanged, fromEvent, merge, tap} from "rxjs";
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {AsyncPipe} from "@angular/common";
import {MatRadioButton, MatRadioGroup} from "@angular/material/radio";
import {MatSort, MatSortModule} from "@angular/material/sort";
import {FormsModule} from "@angular/forms";

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss',
  standalone: true,
  imports: [MatTableModule, MatPaginatorModule, MatLabel, MatFormField, MatInputModule, MatProgressSpinnerModule,
    AsyncPipe, MatRadioButton, MatRadioGroup, MatSortModule, FormsModule]
})
export class OrdersComponent implements AfterViewInit {
  displayedColumns: string[] = ['orderId', 'createdAt', 'coinsAmount', 'tradingPair', 'orderType', 'forecastedPrice'];
  dataSource!: OrdersDataSource;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('searchBox') input!: ElementRef;

  ordersType: string = OrdersType[OrdersType.All];

  constructor(private ordersClient: OrdersClient) {
    this.dataSource = new OrdersDataSource(this.ordersClient);
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    fromEvent(this.input.nativeElement,'keyup')
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        tap(() => {
          this.paginator.pageIndex = 0;
          this.loadOrdersPage();
        })
      )
      .subscribe();

    // setTimeout is used because of ExpressionChangedAfterItHasBeenCheckedError. OrdersDataSource.loadingSubject initially has 'false' value
    setTimeout(() => this.dataSource.loadOrders('',
      (<any>OrdersType)[this.ordersType],
      this.paginator.pageIndex,
      this.paginator.pageSize)
    );

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadOrdersPage()))
      .subscribe();
  }

  loadOrdersPage() {
    this.dataSource.loadOrders(
      this.input.nativeElement.value,
      (<any>OrdersType)[this.ordersType],
      this.paginator.pageIndex,
      this.paginator.pageSize);
  }

  ordersTypeChanged(){
    this.loadOrdersPage();
  }

  protected readonly OrdersType = OrdersType;
}
