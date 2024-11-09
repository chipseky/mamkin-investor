import {CollectionViewer, DataSource} from "@angular/cdk/collections";
import {BehaviorSubject, catchError, finalize, Observable, of} from "rxjs";
import {
  TradesTableItem,
  TradesClient, TradeEventsQuery, PagedDataOfObject
} from "../api-clients";

export class TradeEventsDataSource implements DataSource<any> {

  private ordersSubject = new BehaviorSubject<any[]>([]);
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

  loadTradeEvents(pageIndex: number = 0, pageSize: number = 10) {
    this.loadingSubject.next(true);

    this.tradesClient.getTradeEvents(new TradeEventsQuery({
      page: pageIndex,
      pageSize: pageSize})).pipe(
        catchError(() => of([])),
        finalize(() => this.loadingSubject.next(false))
      ).subscribe(data => {
          let pagedData = data as PagedDataOfObject;
          this.ordersSubject.next(pagedData.items);
          this.totalCount = pagedData.totalCount;
        });
  }
}
