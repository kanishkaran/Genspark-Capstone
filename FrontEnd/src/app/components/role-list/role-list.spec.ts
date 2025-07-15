import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RoleList } from './role-list';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { RoleService } from '../../services/role.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { AccessLevelService } from '../../services/access.service';

describe('RoleList', () => {
  let component: RoleList;
  let fixture: ComponentFixture<RoleList>;

  const mockRoleService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: { data: { $values: [{ id: '1', roleName: 'Admin', accessLevel: 'Full' }] }, totalRecords: 1 }
    })),
    delete: jasmine.createSpy().and.returnValue(of({ message: 'Deleted successfully' })),
    create: jasmine.createSpy().and.returnValue(of({})),
    update: jasmine.createSpy().and.returnValue(of({}))
  };

  const mockDialog = {
    open: jasmine.createSpy().and.returnValue({
      afterClosed: () => of(true)
    })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoleList],
      providers: [
        { provide: RoleService, useValue: mockRoleService },
        { provide: MatDialog, useValue: mockDialog },
        { provide: NotificationService, useValue: { showSuccess: () => {}, showError: () => {} } },
        { provide: AuthService, useValue: { isAdmin: () => true } },
        { provide: AccessLevelService, useValue: { getAll: () => of({ data: { data: { $values: [] } } }) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoleList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should load roles on init', () => {
    expect(component.roles.length).toBe(1);
    expect(component.totalCount).toBe(1);
    expect(mockRoleService.getAll).toHaveBeenCalled();
  });

  it('should call delete dialog and delete role on confirm', () => {
    const role : any = { id: '1', roleName: 'Admin' };
    component.openDeleteDialog(role);
    fixture.detectChanges();

    expect(mockDialog.open).toHaveBeenCalled();
    expect(mockRoleService.delete).toHaveBeenCalledWith('1');
  });

  it('should open role dialog and create new role', () => {
    const newRole = { roleName: 'Editor', accessLevel: 'Write' };
    mockDialog.open = jasmine.createSpy().and.returnValue({
      afterClosed: () => of(newRole)
    });

    component.openRoleDialog(false, null);

    expect(mockDialog.open).toHaveBeenCalled();
    expect(mockRoleService.create).toHaveBeenCalledWith({
      role: 'Editor',
      accessLevel: 'Write'
    });
  });
});
