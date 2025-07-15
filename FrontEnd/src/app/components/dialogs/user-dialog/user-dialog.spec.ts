import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserDialog } from './user-dialog';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

describe('UserDialog', () => {
  let component: UserDialog;
  let fixture: ComponentFixture<UserDialog>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<UserDialog>>;

  const mockRoles = [
    { roleName: 'Admin', accessLevel: 'Full' },
    { roleName: 'User', accessLevel: 'Limited' }
  ];

  beforeEach(async () => {
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);
  });

  it('should initialize create form when not in edit mode', async () => {
    await TestBed.configureTestingModule({
      imports: [UserDialog, FormsModule],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy },
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            roles: mockRoles,
            isEditMode: false,
            formModel: { username: 'newuser', password: 'pass123', roleName: 'User' }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.isEditMode).toBeFalse();
    expect(component.createFormModel.username).toBe('newuser');

    component.onFormSubmit();
    expect(dialogRefSpy.close).toHaveBeenCalledWith(component.createFormModel);
  });

  it('should initialize update form when in edit mode', async () => {
    await TestBed.configureTestingModule({
      imports: [UserDialog, FormsModule],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy },
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            roles: mockRoles,
            isEditMode: true,
            formModel: { username: 'edituser', roleName: 'Admin' }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.isEditMode).toBeTrue();
    expect(component.updateFormModel.username).toBe('edituser');

    component.onFormSubmit();
    expect(dialogRefSpy.close).toHaveBeenCalledWith(component.updateFormModel);
  });

  it('should close dialog with null on cancel', async () => {
    await TestBed.configureTestingModule({
      imports: [UserDialog, FormsModule],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy },
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            roles: mockRoles,
            isEditMode: false,
            formModel: { username: '', password: '', roleName: '' }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();

    component.cancelForm();
    expect(dialogRefSpy.close).toHaveBeenCalledWith(null);
  });
});
