import {CollectionViewer, DataSource} from "@angular/cdk/collections";
import {BehaviorSubject, catchError, finalize, Observable, of} from "rxjs";
import {TradesTableItem, TradesClient, TradesTableDataQuery, TradesTableOrderType, PagedDataOfTradesTableItem} from "../api-clients";

export class TradesDataSource implements DataSource<TradesTableItem> {

  private ordersSubject = new BehaviorSubject<TradesTableItem[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  public loading$ = this.loadingSubject.asObservable();
  public totalCount: number = 0;

  constructor(private tradesClient: TradesClient) {}

  connect(_: CollectionViewer): Observable<TradesTableItem[]> {
    return this.ordersSubject.asObservable();
  }

  disconnect(_: CollectionViewer): void {
    this.ordersSubject.complete();
    this.loadingSubject.complete();
  }

  loadTrades(filter: string = '', ordersType: TradesTableOrderType, pageIndex: number = 0, pageSize: number = 10) {
    this.loadingSubject.next(true);

    this.tradesClient.getTrades(new TradesTableDataQuery({
      tradingPair: filter,
      ordersType: ordersType,
      page: pageIndex,
      pageSize: pageSize})).pipe(
        catchError(() => of([])),
        finalize(() => this.loadingSubject.next(false))
      ).subscribe(data => {
          let pagedData = data as PagedDataOfTradesTableItem;
          this.ordersSubject.next(pagedData.items);
          this.totalCount = pagedData.totalCount;
        });
  }
}
