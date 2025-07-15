import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserList } from './user-list';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { RoleService } from '../../services/role.service';
import { NotificationService } from '../../services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { Router } from '@angular/router';

describe('UserList', () => {
  let component: UserList;
  let fixture: ComponentFixture<UserList>;

  const mockUserService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: {
        data: { $values: [{ username: 'testuser', roleName: 'User', isDeleted: false }] },
        totalRecords: 1
      }
    })),
    updateUserRole: jasmine.createSpy().and.returnValue(of({})),
    delete: jasmine.createSpy().and.returnValue(of({})),
    create: jasmine.createSpy().and.returnValue(of({}))
  };

  const mockRoleService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: { data: { $values: [{ roleName: 'User' }] } }
    }))
  };

  const mockDialog = {
    open: jasmine.createSpy().and.returnValue({ afterClosed: () => of(true) })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserList],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: RoleService, useValue: mockRoleService },
        { provide: AuthService, useValue: { isAdmin: () => true } },
        { provide: NotificationService, useValue: { showSuccess: () => {}, showError: () => {}, showWarning: () => {} } },
        { provide: MatDialog, useValue: mockDialog },
        { provide: Router, useValue: { navigate: jasmine.createSpy() } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should load users and roles on init', () => {
    expect(component.users.length).toBe(1);
    expect(component.roles.length).toBe(1);
    expect(component.total).toBe(1);
    expect(mockUserService.getAll).toHaveBeenCalled();
    expect(mockRoleService.getAll).toHaveBeenCalled();
  });

  it('should open dialog and call updateUserRole', () => {
    const dialogResult = { username: 'testuser', roleName: 'Admin' };
    mockDialog.open = jasmine.createSpy().and.returnValue({ afterClosed: () => of(dialogResult) });

    component.openUserDialog(true, dialogResult);

    expect(mockDialog.open).toHaveBeenCalled();
    expect(mockUserService.updateUserRole).toHaveBeenCalledWith({
      username: 'testuser',
      role: 'Admin'
    });
  });

  it('should delete a user after confirmation', () => {
    const user = { username: 'testuser' } as any;
    component.openDeleteDialog(user);

    expect(mockDialog.open).toHaveBeenCalled();
    expect(mockUserService.delete).toHaveBeenCalledWith('testuser');
  });
});
