export interface SendConfirmationToEmailModel {
  userId: string;
  url: string | null;
  email: string;
  isChange: boolean;
}
