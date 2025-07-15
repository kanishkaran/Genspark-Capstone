import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UpdateProfileDialog } from './update-profile-dialog';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ReactiveFormsModule } from '@angular/forms';

describe('UpdateProfileDialog', () => {
  let component: UpdateProfileDialog;
  let fixture: ComponentFixture<UpdateProfileDialog>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<UpdateProfileDialog>>;

  const mockUserData = {
    firstName: 'John',
    lastName: 'Doe',
    contactNumber: '1234567890',
    email: 'john.doe@example.com'
  };

  beforeEach(async () => {
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [UpdateProfileDialog, ReactiveFormsModule],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: mockUserData }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateProfileDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should initialize form with injected user data', () => {
    expect(component.userForm.value).toEqual(mockUserData);
  });

  it('should close dialog with form data on valid submit', () => {
    component.Submit();
    expect(dialogRefSpy.close).toHaveBeenCalledWith(mockUserData);
  });

  it('should close dialog with null on cancel', () => {
    component.Cancel();
    expect(dialogRefSpy.close).toHaveBeenCalledWith(null);
  });
});
