import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RoleCategoryList } from './role-category-list';
import { of } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { RoleCategoryService } from '../../services/roleCategory.service';
import { RoleService } from '../../services/role.service';
import { CategoryService } from '../../services/category.service';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';

describe('RoleCategoryList', () => {
  let component: RoleCategoryList;
  let fixture: ComponentFixture<RoleCategoryList>;

  const mockRoleCategoryService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: {
        data: { $values: [{ id: '1', roleName: 'Admin', categoryName: 'Finance', canUpload: true, canDownload: true }] },
        totalRecords: 1
      }
    })),
    delete: jasmine.createSpy().and.returnValue(of({ message: 'Deleted successfully' }))
  };

  const mockRoleService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: { data: { $values: [{ roleName: 'Admin' }] } }
    }))
  };

  const mockCategoryService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: { data: { $values: [{ categoryName: 'Finance' }] } }
    }))
  };

  const mockDialog = {
    open: jasmine.createSpy().and.returnValue({ afterClosed: () => of(true) })
  };

  const mockNotificationService = {
    showSuccess: jasmine.createSpy(),
    showError: jasmine.createSpy()
  };

  const mockAuthService = {
    isAdmin: () => true
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoleCategoryList],
      providers: [
        { provide: RoleCategoryService, useValue: mockRoleCategoryService },
        { provide: RoleService, useValue: mockRoleService },
        { provide: CategoryService, useValue: mockCategoryService },
        { provide: MatDialog, useValue: mockDialog },
        { provide: NotificationService, useValue: mockNotificationService },
        { provide: AuthService, useValue: mockAuthService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoleCategoryList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should load role categories and metadata on init', () => {
    expect(component.roleCategories.length).toBe(1);
    expect(component.totalCount).toBe(1);
    expect(mockRoleCategoryService.getAll).toHaveBeenCalled();
    expect(mockRoleService.getAll).toHaveBeenCalled();
    expect(mockCategoryService.getAll).toHaveBeenCalled();
  });

  it('should open delete dialog and delete role-category if confirmed', () => {
    const rc = { id: '1', roleName: 'Admin', categoryName: 'Finance' } as any;
    spyOn(window, 'confirm').and.returnValue(true); // simulate user confirming

    component.openDeleteDialog(rc);

    expect(mockDialog.open).toHaveBeenCalled();
    expect(mockRoleCategoryService.delete).toHaveBeenCalledWith('1');
    expect(mockNotificationService.showSuccess).toHaveBeenCalledWith('Deleted successfully');
  });

});
