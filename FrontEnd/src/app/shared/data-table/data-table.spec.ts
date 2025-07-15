import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component } from '@angular/core';

import { QueryParams } from '../../models/apiResponse';
import { DataTable, TableColumn } from './data-table';


@Component({
  imports: [DataTable],
  template: `
    <app-data-table
      [columns]="columns"
      [data]="data"
      [pageSize]="pageSize"
      [totalCount]="totalCount"
      [isAdmin]="true"
      (queryChanged)="onQueryChanged($event)"
      (addClicked)="onAddClicked()"
    ></app-data-table>
  `
})
class TestHostComponent {
  columns: TableColumn[] = [
    { key: 'name', label: 'Name' },
    { key: 'email', label: 'Email' }
  ];

  data = [
    { name: 'John Doe', email: 'john@example.com' },
    { name: 'Alice Smith', email: 'alice@example.com' }
  ];

  pageSize = 10;
  totalCount = 2;

  queryChangeParams: QueryParams | null = null;
  addClickedCalled = false;

  onQueryChanged(params: QueryParams) {
    this.queryChangeParams = params;
  }

  onAddClicked() {
    this.addClickedCalled = true;
  }
}

describe('DataTable via HostComponent', () => {
  let fixture: ComponentFixture<TestHostComponent>;
  let hostComponent: TestHostComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TestHostComponent);
    hostComponent = fixture.componentInstance;
    fixture.detectChanges();
  });


  it('should react to queryChanged emit from inner table', () => {
    const inner = fixture.debugElement.children[0].componentInstance;
    inner.queryChanged.emit({ search: 'test' });
    expect(hostComponent.queryChangeParams?.search).toBe('test');
  });

  it('should react to addClicked emit from inner table', () => {
    const inner = fixture.debugElement.children[0].componentInstance;
    inner.addClicked.emit();
    expect(hostComponent.addClickedCalled).toBeTrue();
  });
});
