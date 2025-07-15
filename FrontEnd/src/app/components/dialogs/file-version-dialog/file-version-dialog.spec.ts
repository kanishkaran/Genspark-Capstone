import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FileVersionDialog } from './file-version-dialog';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FileArchiveService } from '../../../services/fileUpload.service';
import { NotificationService } from '../../../services/notification.service';
import { of } from 'rxjs';

describe('FileVersionDialog', () => {
  let component: FileVersionDialog;
  let fixture: ComponentFixture<FileVersionDialog>;

  const mockDialogRef = {
    close: jasmine.createSpy('close')
  };

  const mockFileVersions : any = [
    {
      fileName: 'report.docx',
      versionNumber: 1,
      archiveId: '123',
      uploadedBy: 'user1',
      uploadedByName: 'User One',
      createdAt: '2023-01-01T10:00:00Z'
    }
  ];

  const mockData = {
    versions: mockFileVersions
  };

  const mockFileArchiveService = {
    downloadFile: jasmine.createSpy('downloadFile').and.returnValue(of(new Blob(['test'], { type: 'application/octet-stream' })))
  };

  const mockNotificationService = {
    showSuccess: jasmine.createSpy('showSuccess'),
    showError: jasmine.createSpy('showError')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FileVersionDialog],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef },
        { provide: MAT_DIALOG_DATA, useValue: mockData },
        { provide: FileArchiveService, useValue: mockFileArchiveService },
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(FileVersionDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the dialog component', () => {
    expect(component).toBeTruthy();
    expect(component.fileVersions.length).toBe(1);
    expect(component.fileVersions[0].fileName).toBe('report.docx');
  });


  it('should close dialog and clear data', () => {
    component.close();
    expect(mockDialogRef.close).toHaveBeenCalledWith(null);
  });
});
