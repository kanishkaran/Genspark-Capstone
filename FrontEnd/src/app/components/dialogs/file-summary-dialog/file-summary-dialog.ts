import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import {  DatePipe } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { marked } from 'marked'


@Component({
  selector: 'app-file-summary-dialog',
  standalone: true,
  imports: [
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    DatePipe
  ],
  templateUrl: './file-summary-dialog.html',
  styleUrl: './file-summary-dialog.css'
})
export class FileSummaryDialog {
  search: boolean =false;
  rawHtml: any;
  
  constructor(
    public dialogRef: MatDialogRef<FileSummaryDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.search = data.isSearch;
    this.rawHtml = marked(data.summary.summary)
    console.log(data)
  }
  
  public get Time() : number {
    return Date.now()
  }
  
}