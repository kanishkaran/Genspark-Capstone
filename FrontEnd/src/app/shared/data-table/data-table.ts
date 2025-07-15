import { Component, Input, Output, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { QueryParams } from '../../models/apiResponse';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FormsModule } from '@angular/forms';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatCardModule } from '@angular/material/card';



@Component({
  selector: 'app-data-table',
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTooltipModule,
    MatSlideToggleModule,
    FormsModule,
    MatCardModule
  ],
  templateUrl: './data-table.html',
  styleUrl: './data-table.css'
})

export class DataTable implements OnInit {
  @Input() columns: TableColumn[] = [];
  @Input() data: any[] = [];
  @Input() actions: TableAction[] = [];
  @Input() totalCount = 0;
  @Input() pageSize = 10;
  @Input() loading = false;
  @Input() showSearch = true;
  @Input() isAdmin = false;
  @Input() disableAdd: boolean = false;
  @Output() queryChanged = new EventEmitter<QueryParams>();
  @Output() addClicked = new EventEmitter<void>();
  @Output() searchModeToggle = new EventEmitter();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  private searchSubject = new Subject<string>();


  dataSource = new MatTableDataSource<any>([]);
  displayedColumns: string[] = [];
  includeInactive: boolean = false;
  vectorSearch: boolean = false;


  ngOnInit(): void {
    this.dataSource.data = this.data;
    this.displayedColumns = [...this.columns.map(c => c.key)];
    if (this.actions?.length && this.isAdmin) {
      this.displayedColumns.push('actions');
    }

    this.searchSubject
      .pipe(debounceTime(2000), distinctUntilChanged())
      .subscribe(search => {
        this.emitQueryChange({ search });
      });
  }


  ngOnChanges(): void {
    this.dataSource.data = this.data;
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;

    this.sort.sortChange.subscribe((sort: Sort) => {
      this.onSortChange(sort);
    });
  }


  onSearch(event: KeyboardEvent): void {
    const searchTerm = (event.target as HTMLInputElement).value.trim();
    this.searchSubject.next(searchTerm);
  }


  onPageChange(event: PageEvent): void {
    this.emitQueryChange({
      page: event.pageIndex + 1,
      pageSize: event.pageSize
    });
  }

  onSortChange(sort: Sort): void {
    this.emitQueryChange({
      sortBy: sort.active,
      Desc: false
    });
  }

  onIncludeInactiveChange(): void {
    this.emitQueryChange({ IncludeInactive: this.includeInactive });
  }

  onSearchModeChange(){
    console.log(this.vectorSearch)
    this.searchModeToggle.emit(this.vectorSearch);
  }


  onAdd(): void {
    this.addClicked.emit();
  }

  getVisibleActions(): TableAction[] {
    return this.actions
  }

  private emitQueryChange(params: Partial<QueryParams>): void {
    this.queryChanged.emit(params);
  }
}
export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  pipe?: string;
}

export interface TableAction {
  label: string;
  icon: string;
  color?: string;
  action: (item: any) => void;
  showIf: (item: any) => boolean;
}