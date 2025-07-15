import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DeleteDialog } from './delete-dialog';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

describe('DeleteDialog', () => {
  let component: DeleteDialog;
  let fixture: ComponentFixture<DeleteDialog>;
  let mockDialogRef: jasmine.SpyObj<MatDialogRef<DeleteDialog>>;

  beforeEach(async () => {
    mockDialogRef = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [MatDialogModule, DeleteDialog],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef },
        { provide: MAT_DIALOG_DATA, useValue: 'Sample Category' }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DeleteDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should initialize with provided data', () => {
    expect(component.element).toBe('Sample Category');
  });

  it('should close with true on deleteElement()', () => {
    component.deleteElement();
    expect(mockDialogRef.close).toHaveBeenCalledWith(true);
  });

  it('should close with null on cancelDelete()', () => {
    component.cancelDelete();
    expect(mockDialogRef.close).toHaveBeenCalledWith(null);
  });
});
