import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Dashboard } from './dashboard';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { FileArchiveService } from '../../services/fileUpload.service';
import { FileVersionService } from '../../services/fileVersion.service';

describe('Dashboard Component', () => {
  let component: Dashboard;
  let fixture: ComponentFixture<Dashboard>;

  let mockArchiveService = jasmine.createSpyObj('FileArchiveService', ['getAll']);
  let mockVersionService = jasmine.createSpyObj('FileVersionService', ['getAll']);
  let mockRouter = jasmine.createSpyObj('Router', ['navigate']);

  const mockFiles = [
    { id: '1', fileName: 'Test.pdf', categoryName: 'Reports', uploadedByName: 'John', status: false, canSummarise: true },
    { id: '2', fileName: 'Old.docx', categoryName: 'Docs', uploadedByName: 'Jane', status: true, canSummarise: false }
  ];

  const mockVersions = [
    { archiveId: '1', versionNumber: 2, createdAt: new Date('2024-06-01') },
    { archiveId: '2', versionNumber: 1, createdAt: new Date('2024-01-01') }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Dashboard],
      providers: [
        { provide: FileArchiveService, useValue: mockArchiveService },
        { provide: FileVersionService, useValue: mockVersionService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Dashboard);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load data and build summary cards', () => {
    mockArchiveService.getAll.and.returnValue(of({ data: { data: { $values: mockFiles } } }));
    mockVersionService.getAll.and.returnValue(of({ data: { $values: mockVersions } }));

    fixture.detectChanges(); 

    expect(mockArchiveService.getAll).toHaveBeenCalled();
    expect(mockVersionService.getAll).toHaveBeenCalled();

    expect(component.summaryCards[0].value).toBe(2); // total files
    expect(component.summaryCards[1].value).toBe(2); // total versions
    expect(component.summaryCards[2].value).toBe(1); // active files (status false)
    expect(component.summaryCards[3].value).toBe(1); // canSummarise true

    expect(component.recentFiles.length).toBe(2);
    expect(component.recentFiles[0].fileName).toBe('Test.pdf');
  });

  it('should navigate to upload page', () => {
    component.navigateToUpload();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['home', 'upload']);
  });

  it('should navigate to files page', () => {
    component.navigateToFiles();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['home', 'user-category']);
  });
});
