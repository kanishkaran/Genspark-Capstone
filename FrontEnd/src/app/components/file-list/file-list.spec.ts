import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { FileList } from './file-list';
import { FileArchiveService } from '../../services/fileUpload.service';
import { FileVersionService } from '../../services/fileVersion.service';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { FileSummaryService } from '../../services/fileSummary.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';

describe('FileList Component', () => {
  let component: FileList;
  let fixture: ComponentFixture<FileList>;

  const mockArchives = [
    { id: '1', fileName: 'file1.pdf', categoryName: 'Reports', uploadedByName: 'John', status: false, canSummarise: true }
  ];

  const mockVersions = [
    { archiveId: '1', versionNumber: 1, createdAt: new Date() }
  ];

  const mockDialogRef = {
    afterClosed: () => of(true)
  };

  const fileArchiveService = jasmine.createSpyObj('FileArchiveService', ['getAll', 'downloadFile', 'delete']);
  const fileVersionService = jasmine.createSpyObj('FileVersionService', ['getByArchiveId']);
  const notificationService = jasmine.createSpyObj('NotificationService', ['showSuccess', 'showError', 'showInfo']);
  const authService = jasmine.createSpyObj('AuthService', ['isAdmin']);
  const fileSummaryService = jasmine.createSpyObj('FileSummaryService', ['semanticSearch']);
  const dialog = jasmine.createSpyObj('MatDialog', ['open']);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        FileList,
        MatDialogModule,
        CommonModule
      ],
      providers: [
        { provide: FileArchiveService, useValue: fileArchiveService },
        { provide: FileVersionService, useValue: fileVersionService },
        { provide: NotificationService, useValue: notificationService },
        { provide: AuthService, useValue: authService },
        { provide: FileSummaryService, useValue: fileSummaryService },
        { provide: MatDialog, useValue: dialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(FileList);
    component = fixture.componentInstance;
  });

  beforeEach(() => {
    fileArchiveService.getAll.calls.reset();
    fileVersionService.getByArchiveId.calls.reset();
    fileArchiveService.downloadFile.calls.reset();
    fileArchiveService.delete.calls.reset();
    notificationService.showSuccess.calls.reset();
    notificationService.showError.calls.reset();
    notificationService.showInfo.calls.reset();
    authService.isAdmin.and.returnValue(true);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load file archives on init', () => {
    fileArchiveService.getAll.and.returnValue(of({ data: { data: { $values: mockArchives }, totalRecords: 1 } }));

    component.ngOnInit();

    expect(fileArchiveService.getAll).toHaveBeenCalled();
    expect(component.FileArchives.length).toBe(1);
    expect(component.totalCount).toBe(1);
  });

  it('should get file versions and open dialog', () => {
    fileVersionService.getByArchiveId.and.returnValue(of({ data: { $values: mockVersions } }));
    dialog.open.and.returnValue(mockDialogRef as any);

    component.getFileVersionsForArchive(mockArchives[0]);

    expect(fileVersionService.getByArchiveId).toHaveBeenCalledWith('1');
    expect(dialog.open).toHaveBeenCalled();
  });

  it('should show info when no file versions', () => {
    fileVersionService.getByArchiveId.and.returnValue(of({ data: { $values: [] } }));

    component.getFileVersionsForArchive(mockArchives[0]);

    expect(notificationService.showInfo).toHaveBeenCalledWith('No versions available for this file.');
  });

  it('should update query on change', () => {
    fileArchiveService.getAll.and.returnValue(of({ data: { data: { $values: [] }, totalRecords: 0 } }));

    component.onQueryChanged({ page: 2 });

    expect(component.currentQuery.page).toBe(2);
    expect(fileArchiveService.getAll).toHaveBeenCalled();
  });
});
