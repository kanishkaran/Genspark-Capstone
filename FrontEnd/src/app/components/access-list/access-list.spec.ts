import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AccessList } from './access-list';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { AccessLevelService } from '../../services/access.service';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';

describe('AccessList', () => {
  let component: AccessList;
  let fixture: ComponentFixture<AccessList>;

  const mockAccessService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: {
        data: { $values: [{ id: '1', access: 'ReadOnly', isActive: true }] }
      }
    })),
    create: jasmine.createSpy().and.returnValue(of({})),
    delete: jasmine.createSpy().and.returnValue(of({ data: 'Access deleted' }))
  };

  const mockAuthService = {
    isAdmin: () => true
  };

  const mockNotification = {
    showSuccess: jasmine.createSpy(),
    showError: jasmine.createSpy()
  };

  const mockDialog = {
    open: jasmine.createSpy().and.returnValue({ afterClosed: () => of(true) })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccessList],
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: NotificationService, useValue: mockNotification },
        { provide: AccessLevelService, useValue: mockAccessService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AccessList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should load access levels on init', () => {
    expect(component.accessLevels.length).toBe(1);
    expect(mockAccessService.getAll).toHaveBeenCalled();
    expect(component.accessLevels[0].access).toBe('ReadOnly');
  });

  it('should open access creation dialog and create access on result', () => {
    const result = { access: 'Admin' };
    mockDialog.open = jasmine.createSpy().and.returnValue({ afterClosed: () => of(result) });

    component.openAccessDialog();

    expect(mockDialog.open).toHaveBeenCalled();
    expect(mockAccessService.create).toHaveBeenCalledWith(result);
    expect(mockNotification.showSuccess).toHaveBeenCalledWith('Access Level Created Successfully');
  });

  it('should delete access after confirmation', () => {
    const access = { id: '1', access: 'ReadOnly', isActive: true } as any;

    component.openDeleteDialog(access);

    expect(mockDialog.open).toHaveBeenCalled();
    expect(mockAccessService.delete).toHaveBeenCalledWith('1');
    expect(mockNotification.showSuccess).toHaveBeenCalledWith('Access deleted');
  });
});
