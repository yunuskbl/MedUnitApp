export interface SertifikaModel {
  baslik: string;
  kurum: string;
  yil: number;
  ikon: string;
}

export interface CalismaSaatiModel {
  gun: string;
  baslangicSaati: string;
  bitisSaati: string;
  izinli: boolean;
}

export interface ProfilModel {
  adSoyad: string;
  biyografi: string;
  danismanlikUcreti: number;
  uzmanlikAlanlari: string[];
  sertifikalar: SertifikaModel[];
  calismaSaatleri: CalismaSaatiModel[];
}