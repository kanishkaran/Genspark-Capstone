import { Component, OnInit } from '@angular/core';
import { FileArchiveService } from '../../services/fileUpload.service';
import { FileVersionService } from '../../services/fileVersion.service';
import { FileArchive } from '../../models/fileArchive.model';
import { FileVersion } from '../../models/fileVersion.model';

import { CommonModule, DatePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatBadgeModule } from '@angular/material/badge';
import { Router } from '@angular/router';
import { NgApexchartsModule } from 'ng-apexcharts'

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatCardModule, MatTableModule, MatBadgeModule, DatePipe, NgApexchartsModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class Dashboard implements OnInit {
  recentFiles: any[] = [];
  files: FileArchive[] = [];
  versions: FileVersion[] = [];
  activityChartData: any;
  categoryChartData: { labels: string[], values: number[] } = { labels: [], values: [] };
  versionStats: number[] = [];
  recentDates: any[] = [];


  plotOptions: any = {
    pie: {
      donut: {
        size: '65%',
        labels: {
          show: true,
          total: {
            show: true,
            label: 'Total Categories',
            formatter: () => {
              return this.categoryChartData.labels.length.toString();
            }
          }
        }
      }
    }
  };



  summaryCards = [
    { title: 'Total Files', value: 0, icon: 'folder', color: 'bg-primary' },
    { title: 'Total Versions', value: 0, icon: 'layers', color: 'bg-info' },
    { title: 'Active Files', value: 0, icon: 'category', color: 'bg-success' },
    { title: 'Summarisable Files', value: 0, icon: 'summarize', color: 'bg-danger' },
  ];
  versionPlotOptions: any = {
    radialBar: {
      dataLabels: {
        name: {
          fontSize: '16px',
          fontWeight: 600
        },
        value: {
          fontSize: '14px',
          fontWeight: 500
        },
        total: {
          show: true,
          label: 'Files with Versions',
          formatter: () => {
            return  this.versions.filter(v => v.versionNumber > 1).length
          }
        }
      }
    }
  }

  displayedColumns: string[] = [
    'fileName',
    'categoryName',
    'uploadedByName',
    'status',
    'latestVersion',
    'canSummarise'
  ];

  constructor(
    private fileArchiveService: FileArchiveService,
    private fileVersionService: FileVersionService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadDashboardData();
    this.recentDates = this.getRecentDates();
  }

  private loadDashboardData(): void {
    this.fileArchiveService.getAll({ page: 1, pageSize: 100, IncludeInactive: true }).subscribe((archiveRes: any) => {
      this.files = archiveRes.data.data?.$values || [];

      this.summaryCards[0].value = this.files.length;
      this.summaryCards[2].value = this.files.filter(f => !f.status).length;
      this.summaryCards[3].value = this.files.filter(f => f.canSummarise).length;

      this.fileVersionService.getAll().subscribe((versionRes: any) => {
        this.versions = versionRes.data?.$values || [];
        this.summaryCards[1].value = this.versions.length;

        this.buildRecentFiles();
        if (this.versions.length > 0){
          this.activityChartData = this.getActivityData();
          this.versionStats = this.getVersionStats();
        }
      });
      if(this.files.length > 0){
        this.categoryChartData = this.getCategoryData();
        console.log("category data", this.categoryChartData)
      }
    });
  }

  private buildRecentFiles(): void {
    const enrichedFiles = this.files.map(file => {
      const versions = this.versions
        .filter(v => v.archiveId === file.id)
        .sort((a, b) => b.versionNumber - a.versionNumber);

      return {
        ...file,
        latestVersion: versions[0] || null
      };
    });

    this.recentFiles = enrichedFiles
      .filter(f => f.latestVersion)
      .sort((a, b) => {
        const aDate = new Date(a.latestVersion.createdAt || 0).getTime();
        const bDate = new Date(b.latestVersion.createdAt || 0).getTime();
        return bDate - aDate;
      })
      .slice(0, 10);
  }

  getCategoryData(): { labels: string[], values: number[] } {
    const categoryMap = new Map<string, number>();

    this.files.forEach(file => {
      const category = file.categoryName || 'Uncategorized';
      categoryMap.set(category, (categoryMap.get(category) || 0) + 1);
    });

    return {
      labels: Array.from(categoryMap.keys()),
      values: Array.from(categoryMap.values())
    };
  }

  getVersionStats(): number[] {
    const totalFiles = this.files.length;
    if (totalFiles === 0) return [0, 0, 0];

    const filesWithVersions = this.files.filter(file =>
      this.versions.some(version => version.archiveId === file.id && version.versionNumber > 1)
    ).length;

    const summarisableFiles = this.files.filter(f => f.canSummarise).length;
    const activeFiles = this.files.filter(f => !f.status).length;

    return [
      Math.round((filesWithVersions / totalFiles) * 100),
      Math.round((summarisableFiles / totalFiles) * 100),
      Math.round((activeFiles / totalFiles) * 100)
    ];
  }

  getActivityData(): { name: string, data: number[] }[] {
    const last7Days = this.getRecentDates();
    const activityData = last7Days.map(date => {
      const dayFiles = this.versions.filter(file => {
        const fileDate = new Date(file.createdAt || '').toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
        return fileDate === new Date(date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' }) && file.versionNumber == 1;
      });
      return dayFiles.length;
    });

    return [{
      name: 'Files Uploaded',
      data: activityData
    }];
  }

  getRecentDates(): string[] {
    const dates = [];
    for (let i = 29; i >= 0; i--) {
      const date = new Date();
      date.setDate(date.getDate() - i);
      dates.push(date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }));
    }
    return dates;
  }


  navigateToUpload() {
    this.router.navigate(['home', 'upload'])
  }

  navigateToFiles() {
    this.router.navigate(['home', 'user-category'])
  }
}
