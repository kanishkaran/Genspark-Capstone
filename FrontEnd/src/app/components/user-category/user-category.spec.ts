import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { UserCategory } from './user-category';
import { CategoryService } from '../../services/category.service';
import { RoleCategoryService } from '../../services/roleCategory.service';
import { FileArchiveService } from '../../services/fileUpload.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { MatDialogModule } from '@angular/material/dialog';

describe('UserCategoryComponent', () => {
  let component: UserCategory;
  let fixture: ComponentFixture<UserCategory>;

  const mockCategories = [
    { id: '1', categoryName: 'Cat1', access: 'Admin' },
    { id: '2', categoryName: 'Cat2', access: 'Write' }
  ];

  const mockFiles = [
    { id: 'f1', fileName: 'file1.pdf', categoryName: 'Cat1', uploadedByName: 'user', status: true, canSummarise: false },
    { id: 'f2', fileName: 'file2.pdf', categoryName: 'Cat2', uploadedByName: 'user', status: true, canSummarise: true }
  ];

  const mockCategoryService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: { data: { $values: mockCategories } }
    }))
  };

  const mockRoleCategoryService = {
    getAll: jasmine.createSpy().and.returnValue(of({ data: { data: { $values: [] } } }))
  };

  const mockFileService = {
    getAll: jasmine.createSpy().and.returnValue(of({
      data: { data: { $values: mockFiles } }
    })),
    downloadFile: jasmine.createSpy().and.returnValue(of(new Blob(['dummy'], { type: 'application/pdf' })))
  };

  const mockAuthService = {
    currUserSubject: {
      value: {
        username: 'admin@example.com',
        roleName: 'Admin'
      }
    }
  };

  const mockNotificationService = {
    showSuccess: jasmine.createSpy(),
    showError: jasmine.createSpy()
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatDialogModule],
      providers: [
        { provide: CategoryService, useValue: mockCategoryService },
        { provide: RoleCategoryService, useValue: mockRoleCategoryService },
        { provide: FileArchiveService, useValue: mockFileService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserCategory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should create component and load categories and files', () => {
    expect(component).toBeTruthy();
    expect(mockCategoryService.getAll).toHaveBeenCalled();
    expect(mockFileService.getAll).toHaveBeenCalled();
    expect(component.myAuthorisedCategories.length).toBe(2);
    expect(component.files.length).toBe(2);
  });

  it('should filter files by selected category', () => {
    component.files = mockFiles;
    component.openCategory({ categoryName: 'Cat1', access: 'Admin' } as any);
    expect(component.selectedCategory.categoryName).toBe('Cat1');
    expect(component.filesInCategory.length).toBe(1);
    expect(component.filesInCategory[0].fileName).toBe('file1.pdf');
  });
});
