import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CategoryList } from './category-list';
import { of, throwError } from 'rxjs';
import { CategoryService } from '../../services/category.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { AccessLevelService } from '../../services/access.service';
import { MatDialog } from '@angular/material/dialog';

describe('CategoryList', () => {
  let component: CategoryList;
  let fixture: ComponentFixture<CategoryList>;
  let mockCategoryService: jasmine.SpyObj<CategoryService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;
  let mockAccessLevelService: jasmine.SpyObj<AccessLevelService>;
  let mockDialog: jasmine.SpyObj<MatDialog>;

  beforeEach(async () => {
    mockCategoryService = jasmine.createSpyObj('CategoryService', ['getAll', 'create', 'update', 'delete']);
    mockAuthService = jasmine.createSpyObj('AuthService', ['isAdmin']);
    mockNotificationService = jasmine.createSpyObj('NotificationService', ['showError', 'showSuccess']);
    mockAccessLevelService = jasmine.createSpyObj('AccessLevelService', ['getAll']);
    mockDialog = jasmine.createSpyObj('MatDialog', ['open']);

    await TestBed.configureTestingModule({
      imports: [CategoryList],
      providers: [
        { provide: CategoryService, useValue: mockCategoryService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: NotificationService, useValue: mockNotificationService },
        { provide: AccessLevelService, useValue: mockAccessLevelService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryList);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });
  
  it('should handle category load error', () => {
    mockCategoryService.getAll.and.returnValue(throwError(() => new Error('Failed to load')));
    component.loadCategories();

    expect(mockNotificationService.showError).toHaveBeenCalledWith('Failed to load categories');
    expect(component.loading).toBeFalse();
  });

  it('should update query and reload categories', () => {
    const mockResponse: any = {
      data: {
        data: { $values: [{ id: 1, categoryName: 'Updated' }] },
        totalRecords: 1
      }
    };
    mockCategoryService.getAll.and.returnValue(of(mockResponse));

    component.onQueryChanged({ page: 2 });

    expect(component.currentQuery.page).toBe(2);
    expect(mockCategoryService.getAll).toHaveBeenCalled();
    expect(component.categories.length).toBe(1);
  });
});
