import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RoleDialog } from './role-dialog';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

describe('RoleDialog', () => {
  let component: RoleDialog;
  let fixture: ComponentFixture<RoleDialog>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<RoleDialog>>;

  const mockData = {
    formModel: { roleName: 'Admin', accessLevel: 'Full' },
    accessLevels: ['Full', 'Read-only'],
    isEditMode: true
  };

  beforeEach(async () => {
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [RoleDialog, FormsModule],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: mockData }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RoleDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should initialize form with injected data', () => {
    expect(component.formModel.roleName).toBe('Admin');
    expect(component.formModel.accessLevel).toBe('Full');
    expect(component.accessLevels.length).toBe(2);
    expect(component.isEditMode).toBeTrue();
  });

  it('should close dialog with form data on valid submit', () => {
    component.onFormSubmit();
    expect(dialogRefSpy.close).toHaveBeenCalledWith(component.formModel);
  });

  it('should cancel and close dialog with null', () => {
    component.cancelForm();
    expect(dialogRefSpy.close).toHaveBeenCalledWith(null);
  });
});
