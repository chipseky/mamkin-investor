import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {MatPaginator} from "@angular/material/paginator";
import {MatSort} from "@angular/material/sort";
import {TradesClient} from "../api-clients";
import {merge, tap} from "rxjs";
import {TradeEventsDataSource} from "../datasources/trade-events-data-source";
import {AsyncPipe, JsonPipe} from "@angular/common";
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow, MatRowDef, MatTable
} from "@angular/material/table";
import {MatProgressSpinner} from "@angular/material/progress-spinner";

@Component({
  selector: 'app-trade-events',
  standalone: true,
  imports: [
    AsyncPipe,
    MatCell,
    MatCellDef,
    MatColumnDef,
    MatHeaderCell,
    MatHeaderRow,
    MatHeaderRowDef,
    MatPaginator,
    MatProgressSpinner,
    MatRow,
    MatRowDef,
    MatSort,
    MatTable,
    MatHeaderCellDef,
    JsonPipe
  ],
  templateUrl: './trade-events.component.html',
  styleUrl: './trade-events.component.scss'
})
export class TradeEventsComponent implements AfterViewInit {
  displayedColumns: string[] = ['data'];
  dataSource!: TradeEventsDataSource;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private tradesClient: TradesClient) {
    this.dataSource = new TradeEventsDataSource(this.tradesClient);
  }

  ngAfterViewInit() {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    // setTimeout is used because of ExpressionChangedAfterItHasBeenCheckedError. OrdersDataSource.loadingSubject initially has 'false' value
    setTimeout(() =>
      this.dataSource.loadTradeEvents(this.paginator.pageIndex, this.paginator.pageSize));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(tap(() => this.loadTradeEventsPage()))
      .subscribe();
  }

  loadTradeEventsPage() {
    this.dataSource.loadTradeEvents(this.paginator.pageIndex, this.paginator.pageSize);
  }

  protected readonly JSON = JSON;
}
