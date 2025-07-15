import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MediaTypeDialog } from './media-type-dialog';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

describe('MediaTypeDialog', () => {
  let component: MediaTypeDialog;
  let fixture: ComponentFixture<MediaTypeDialog>;

  const mockDialogRef = {
    close: jasmine.createSpy('close')
  };

  const mockData = {
    formModel: { typeName: 'PDF', extension: '.pdf' },
    isEditMode: true
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MediaTypeDialog, FormsModule],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef },
        { provide: MAT_DIALOG_DATA, useValue: mockData }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MediaTypeDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the dialog and populate formModel', () => {
    expect(component).toBeTruthy();
    expect(component.formModel.typeName).toBe('PDF');
    expect(component.formModel.extension).toBe('.pdf');
    expect(component.isEditMode).toBeTrue();
  });

  it('should call dialogRef.close with formModel on valid submit', () => {
    component.onFormSubmit();
    expect(mockDialogRef.close).toHaveBeenCalledWith({ typeName: 'PDF', extension: '.pdf' });
  });

  it('should call dialogRef.close with null on cancel', () => {
    component.cancelForm();
    expect(mockDialogRef.close).toHaveBeenCalledWith(null);
  });
});
