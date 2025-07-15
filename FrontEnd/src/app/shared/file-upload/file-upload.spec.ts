import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FileUpload } from './file-upload';
import {  HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CategoryService } from '../../services/category.service';
import { NotificationService } from '../../services/notification.service';
import { FileArchiveService } from '../../services/fileUpload.service';
import { AuthService } from '../../services/auth.service';
import { provideHttpClient } from '@angular/common/http';

describe('FileUpload Component', () => {
  let component: FileUpload;
  let fixture: ComponentFixture<FileUpload>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FileUpload, ],
      providers: [
        CategoryService,
        FileArchiveService,
        NotificationService,
        {
          provide: AuthService,
          useValue: {
            isAdmin: () => true
          }
        },
        provideHttpClient(), provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(FileUpload);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);

    fixture.detectChanges(); 
  });

  afterEach(() => {
    httpMock.verify(); 
  });

  it('should create component', () => {
    httpMock.expectOne("http://localhost:5125/api/v1/Category")
    expect(component).toBeTruthy();
  });

  it('should fetch categories on init if not specific category', () => {
    const req = httpMock.expectOne(req => req.url.includes('/api/v1/Category'));
    expect(req.request.method).toBe('GET');

    
    req.flush({
      data: {
        data: {
          $values: [
            { categoryName: 'HR', access: 'Write' },
            { categoryName: 'Finance', access: 'read-only' }
          ]
        }
      }
    });

    expect(component.categories.length).toBe(2);
    expect(component.categories[0].categoryName).toBe('HR');
  });
});
