

export interface FileVersion{
    id: string
    archiveId: string
    fileName: string
    versionNumber: number
    contentType: string
    createdAt: Date
    createdBy: string
}

export interface LatestFileInfo {
  archiveId: string;
  fileName: string;
  categoryName: string;
  status: boolean;
  versionNumber: number;
  contentType: string;
  createdAt: Date;
  createdBy: string;
}
