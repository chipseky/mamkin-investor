import {CollectionViewer, DataSource} from "@angular/cdk/collections";
import {BehaviorSubject, catchError, finalize, Observable, of} from "rxjs";
import {Order, OrdersClient, OrdersTableDataQuery, OrdersType, PagedDataOfOrder} from "../api-clients";

export class OrdersDataSource implements DataSource<Order> {

  private ordersSubject = new BehaviorSubject<Order[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  public loading$ = this.loadingSubject.asObservable();
  public totalCount: number = 0;

  constructor(private ordersClient: OrdersClient) {}

  connect(_: CollectionViewer): Observable<Order[]> {
    return this.ordersSubject.asObservable();
  }

  disconnect(_: CollectionViewer): void {
    this.ordersSubject.complete();
    this.loadingSubject.complete();
  }

  loadOrders(filter: string = '', ordersType: OrdersType, pageIndex: number = 0, pageSize: number = 10) {
    this.loadingSubject.next(true);

    this.ordersClient.getOrders(new OrdersTableDataQuery({
      tradingPair: filter,
      ordersType: ordersType,
      page: pageIndex,
      pageSize: pageSize})).pipe(
        catchError(() => of([])),
        finalize(() => this.loadingSubject.next(false))
      ).subscribe(data => {
          let pagedData = data as PagedDataOfOrder;
          this.ordersSubject.next(pagedData.items);
          this.totalCount = pagedData.totalCount;
        });
  }
}
