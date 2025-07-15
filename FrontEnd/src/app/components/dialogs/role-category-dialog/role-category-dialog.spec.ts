import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RoleCategoryDialog } from './role-category-dialog';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

describe('RoleCategoryDialog', () => {
  let component: RoleCategoryDialog;
  let fixture: ComponentFixture<RoleCategoryDialog>;

  const mockDialogRef = {
    close: jasmine.createSpy('close')
  };

  const mockData = {
    formModel: {
      roleName: 'Manager',
      categoryName: 'Invoices',
      canUpload: true,
      canDownload: false
    },
    roles: [{ id: '1', roleName: 'Manager' }],
    categories: [{ id: '1', categoryName: 'Invoices' }],
    isEditMode: true
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoleCategoryDialog, FormsModule],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef },
        { provide: MAT_DIALOG_DATA, useValue: mockData }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoleCategoryDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load data correctly', () => {
    expect(component).toBeTruthy();
    expect(component.formModel.roleName).toBe('Manager');
    expect(component.formModel.categoryName).toBe('Invoices');
    expect(component.isEditMode).toBeTrue();
  });

  it('should call dialogRef.close with formModel on valid submit', () => {
    component.onFormSubmit();
    expect(mockDialogRef.close).toHaveBeenCalledWith(component.formModel);
  });

  it('should close dialog with null on cancel', () => {
    component.cancelForm();
    expect(mockDialogRef.close).toHaveBeenCalledWith(null);
  });
});
