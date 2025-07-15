import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MediaTypesList } from './media-types-list';
import { MediaTypeService } from '../../services/mediaType.service';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';


const mockMediaTypeService = {
  getAll: jasmine.createSpy('getAll').and.returnValue(of({
    data: { data: { $values: [{ id: '1', typeName: 'PDF', extension: '.pdf' }] }, totalRecords: 1 }
  })),
  delete: jasmine.createSpy('delete').and.returnValue(of({ message: 'Deleted successfully' })),
};

const mockNotificationService = {
  showSuccess: jasmine.createSpy('showSuccess'),
  showError: jasmine.createSpy('showError')
};

const mockAuthService = {
  isAdmin: () => true
};

const mockDialog = {
  open: jasmine.createSpy('open').and.returnValue({ afterClosed: () => of(true) })
};

describe('MediaTypesList', () => {
  let component: MediaTypesList;
  let fixture: ComponentFixture<MediaTypesList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MediaTypesList],
      providers: [
        { provide: MediaTypeService, useValue: mockMediaTypeService },
        { provide: NotificationService, useValue: mockNotificationService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: MatDialog, useValue: mockDialog }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MediaTypesList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the MediaTypesList component', () => {
    expect(component).toBeTruthy();
  });

  it('should load media types on init', () => {
    expect(component.mediaTypes.length).toBeGreaterThan(0);
    expect(component.totalCount).toBe(1);
    expect(mockMediaTypeService.getAll).toHaveBeenCalled();
  });

  it('should delete a media type and reload list', () => {
    const sampleType = { id: '1', typeName: 'PDF', extension: '.pdf' };
    component.deleteMediaType(sampleType as any);

    expect(mockMediaTypeService.delete).toHaveBeenCalledWith('1');
    expect(mockNotificationService.showSuccess).toHaveBeenCalledWith(jasmine.stringMatching(/Media Type has been deleted/i));
    expect(mockMediaTypeService.getAll).toHaveBeenCalled();
  });
});
