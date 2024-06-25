import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {MatTableModule} from "@angular/material/table";
import {TradesClient, TradeState} from "../api-clients";
import {MatPaginator, MatPaginatorModule} from "@angular/material/paginator";
import {MatFormField, MatLabel} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {TradesDataSource} from "../datasources/trades-data-source";
import {debounceTime, distinctUntilChanged, fromEvent, merge, tap} from "rxjs";
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {AsyncPipe, JsonPipe, NgClass } from "@angular/common";
import {MatRadioButton, MatRadioGroup} from "@angular/material/radio";
import {MatSort, MatSortModule} from "@angular/material/sort";
import {FormsModule} from "@angular/forms";
import {animate, state, style, transition, trigger} from "@angular/animations";
import {MatIconButton} from "@angular/material/button";
import {MatIcon} from "@angular/material/icon";

@Component({
  selector: 'app-orders',
  templateUrl: './trades.component.html',
  styleUrl: './trades.component.scss',
  standalone: true,
  animations: [
    trigger('detailExpand', [
      state('collapsed,void', style({height: '0px', minHeight: '0'})),
      state('expanded', style({height: '*'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
  imports: [MatTableModule, MatPaginatorModule, MatLabel, MatFormField, MatInputModule, MatProgressSpinnerModule,
    AsyncPipe, MatRadioButton, MatRadioGroup, MatSortModule, FormsModule, MatIconButton, MatIcon, JsonPipe, NgClass],
})
export class TradesComponent implements AfterViewInit {
  displayedColumns: string[] = ['indicator', 'tradeId', 'createdAt', 'coinsAmount', 'tradingPair', 'state'];
  columnsToDisplayWithExpand = [...this.displayedColumns, 'expand'];
  expandedElement: any;
  dataSource!: TradesDataSource;
  protected readonly TradeState = TradeState;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild('searchBox') input!: ElementRef;

  tradeState: string  = '';

  constructor(private tradesClient: TradesClient) {
    this.dataSource = new TradesDataSource(this.tradesClient);
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    fromEvent(this.input.nativeElement,'keyup')
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        tap(() => {
          this.paginator.pageIndex = 0;
          this.loadTradesPage();
        })
      )
      .subscribe();

    // setTimeout is used because of ExpressionChangedAfterItHasBeenCheckedError. OrdersDataSource.loadingSubject initially has 'false' value
    setTimeout(() => this.dataSource.loadTrades('',
      this.tradeState == null ? null : (<any>TradeState)[this.tradeState],
      this.paginator.pageIndex,
      this.paginator.pageSize)
    );

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadTradesPage()))
      .subscribe();
  }

  loadTradesPage() {
    this.dataSource.loadTrades(
      this.input.nativeElement.value,
      this.tradeState == null ? null : (<any>TradeState)[this.tradeState],
      this.paginator.pageIndex,
      this.paginator.pageSize);
  }

  ordersTypeChanged(){
    this.loadTradesPage();
  }
}
