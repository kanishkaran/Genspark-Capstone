export interface FileArchive {
  id: string;
  fileName: string;
  categoryName: string;
  uploadedByName: string;
  status: boolean;
  canSummarise: boolean
}

export interface FileUploadRequest {
  file: File;
  category: string;
}

export interface FileSummary{
  fileName: string
  summary: string
}

export interface semanticSearchFile {
  fileName: string
  fileSummary: string
  confidenceScore: number
}