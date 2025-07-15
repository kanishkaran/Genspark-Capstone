import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AccessDialog } from './access-dialog';
import { MatDialogRef } from '@angular/material/dialog';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { By } from '@angular/platform-browser';

describe('AccessDialog', () => {
  let component: AccessDialog;
  let fixture: ComponentFixture<AccessDialog>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<AccessDialog>>;

  beforeEach(async () => {
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [
        ReactiveFormsModule,
        FormsModule,
        MatInputModule,
        MatFormFieldModule
      ],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AccessDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the dialog component', () => {
    expect(component).toBeTruthy();
  });

  it('should have invalid form initially', () => {
    expect(component.accessForm.valid).toBeFalse();
  });

  it('should not close dialog if form is invalid on submit', () => {
    component.access.setValue('');
    component.onFormSubmit();
    expect(dialogRefSpy.close).not.toHaveBeenCalled();
  });

  it('should close dialog with form value on valid submit', () => {
    component.access.setValue('Write');
    component.onFormSubmit();
    expect(dialogRefSpy.close).toHaveBeenCalledWith({ access: 'Write' });
  });

});
