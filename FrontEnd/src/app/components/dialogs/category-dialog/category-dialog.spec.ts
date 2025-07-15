import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CategoryDialog } from './category-dialog';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

describe('CategoryDialog', () => {
  let component: CategoryDialog;
  let fixture: ComponentFixture<CategoryDialog>;
  let mockDialogRef: jasmine.SpyObj<MatDialogRef<CategoryDialog>>;

  beforeEach(async () => {
    mockDialogRef = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef },
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            formModel: { categoryName: 'Reports', accessLevel: 'Admin' },
            accessLevels: ['Admin', 'Write', 'Read'],
            isEditMode: true
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should initialize with passed data', () => {
    expect(component.formModel.categoryName).toBe('Reports');
    expect(component.accessLevels.length).toBe(3);
    expect(component.isEditMode).toBeTrue();
  });

  it('should close dialog with form data on submit', () => {
    component.onFormSubmit();
    expect(mockDialogRef.close).toHaveBeenCalledWith(component.formModel);
  });

  it('should close dialog with null on cancel', () => {
    component.cancelForm();
    expect(mockDialogRef.close).toHaveBeenCalledWith(null);
  });
});
