import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Profile } from './profile';
import { AuthService } from '../../services/auth.service';
import { EmployeeService } from '../../services/employee.service';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../services/notification.service';
import { MatDialogModule } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';

describe('ProfileComponent', () => {
  let component: Profile;
  let fixture: ComponentFixture<Profile>;
  let authStub: AuthService

  const mockEmployeeService = {
    getEmployeeByEmail: jasmine.createSpy(),
    create: jasmine.createSpy(),
    update: jasmine.createSpy()
  };

  const mockUserService = {
    getById: jasmine.createSpy(),
    changePassword: jasmine.createSpy()
  };

  const mockNotificationService = {
    showWarning: jasmine.createSpy(),
    showSuccess: jasmine.createSpy(),
    showError: jasmine.createSpy(),
    showInfo: jasmine.createSpy()
  };

  const mockActivatedRoute = {
    snapshot: {
      params: {}
    }
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatDialogModule],
      providers: [
        {
          provide: AuthService, useValue: {
            currUser$: of({ username: 'john@example.com', roleName: 'Admin' }),
            isAdmin: () => true
          }
        },
        { provide: EmployeeService, useValue: mockEmployeeService },
        { provide: UserService, useValue: mockUserService },
        { provide: NotificationService, useValue: mockNotificationService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();
    authStub = TestBed.inject(AuthService);
  });


  it('should create and call employeeService if admin', () => {
    mockEmployeeService.getEmployeeByEmail.and.returnValue(
      of({ data: { firstName: 'John', lastName: 'Doe', email: 'john@example.com' } })
    );
    mockUserService.getById.and.returnValue(
      of({ data: { roleName: 'Admin' } })
    );

    fixture = TestBed.createComponent(Profile);
    component = fixture.componentInstance;


    component.userName = 'john@example.com';
    fixture.detectChanges()

    expect(component).toBeTruthy();
    expect(mockEmployeeService.getEmployeeByEmail).toHaveBeenCalled();
  });


  it('should show warning if not admin and not same user', () => {
    spyOn(authStub, 'isAdmin').and.returnValue(false);
    component = TestBed.createComponent(Profile).componentInstance;
    component.userName = 'other@example.com';
    component.ngOnInit();

    expect(mockEmployeeService.getEmployeeByEmail).not.toHaveBeenCalled();
  });
});
