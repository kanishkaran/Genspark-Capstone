import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserFiles } from './user-files';
import { of } from 'rxjs';
import { FileArchiveService } from '../../services/fileUpload.service';
import { FileVersionService } from '../../services/fileVersion.service';
import { EmployeeService } from '../../services/employee.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { FileSummaryService } from '../../services/fileSummary.service';
import { MatDialogModule } from '@angular/material/dialog';
import { Router } from '@angular/router';

describe('UserFiles Component', () => {
  let component: UserFiles;
  let fixture: ComponentFixture<UserFiles>;

  const mockUser = { username: 'test@example.com', roleName: 'User' };
  const mockEmployee = { id: 'emp1', firstName: 'John', email: 'test@example.com' };
  const mockArchives = [
    { id: 'f1', fileName: 'file1.pdf', uploadedByName: 'John', status: false },
    { id: 'f2', fileName: 'file2.docx', uploadedByName: 'NotJohn', status: false },
    { id: 'f3', fileName: 'file3.pdf', uploadedByName: 'John', status: true }
  ];

  const mockAuthService = {
    currUser$: of(mockUser)
  };

  const mockEmployeeService = {
    getAllEmployee: jasmine.createSpy().and.returnValue(of({
      data: { data: { $values: [mockEmployee] } }
    }))
  };

  const mockFileService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: { data: { $values: mockArchives } }
    })),
    downloadFile: jasmine.createSpy().and.returnValue(of(new Blob(['dummy'], { type: 'application/pdf' })))
  };

  const mockFileVersionService = {
    getByArchiveId: jasmine.createSpy()
  };

  const mockFileSummaryService = {
    getFileSummary: jasmine.createSpy()
  };

  const mockNotificationService = {
    showSuccess: jasmine.createSpy(),
    showError: jasmine.createSpy(),
    showInfo: jasmine.createSpy()
  };

  const mockRouter = {
    navigate: jasmine.createSpy()
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatDialogModule],
      providers: [
        { provide: FileArchiveService, useValue: mockFileService },
        { provide: FileVersionService, useValue: mockFileVersionService },
        { provide: FileSummaryService, useValue: mockFileSummaryService },
        { provide: EmployeeService, useValue: mockEmployeeService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: NotificationService, useValue: mockNotificationService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserFiles);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should create and load employee and user file archives', () => {
    expect(component).toBeTruthy();
    expect(mockEmployeeService.getAllEmployee).toHaveBeenCalledWith({ email: mockUser.username });
    expect(mockFileService.getAll).toHaveBeenCalled();

    expect(component.currEmployee.firstName).toBe('John');
    expect(component.myFileArchives.length).toBe(1);
    expect(component.myFileArchives[0].fileName).toBe('file1.pdf');
  });

  it('should navigate to upload page', () => {
    component.navigate();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['home', 'upload']);
  });
});
